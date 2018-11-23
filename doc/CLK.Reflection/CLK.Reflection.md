#[CLK Framework] CLK.Reflection - 極簡風格的依賴注入模組(DI Framework)#


##問題情景##

IoC模式在近代軟體設計中已經成了顯學。不管是將系統設計為抽換邊界物件、或者是將物件設計為能夠自動測試...等等，都可以看到IoC模式的身影。而在系統內加入IoC模式來隔離系統物件與外部物件的相依時，最終大多會透過各種DI Framework的物件生成功能來完成依賴注入的設計，用以在執行期間決定生成何種外部物件來提供系統使用。

一般業界常見的DI Framework有：MEF、Unity、Spring、Autofac等等，這些DI Framework的功能強大，幾乎能夠滿足所有依賴注入的相關設計。但是，也因為這些DI Framework的功能強大，許多額外的功能也被加到Framework的設計之中，例如：物件獨體生成、物件組合生成、物件生命管理等等，這些額外加入Framework的功能，提供開發人員設計系統時的便利，但是也大大提高了學習的門檻。光是搞懂每個DI Framework的設計細節，就需要消耗掉開發人員大量的腦力與時間。

單純想要使用DI Framework的物件反射生成功能來完成依賴注入的設計，是否有簡單的依賴注入模組(DI Framework)呢?


##解決方案##

一般業界常見的DI Framework，可以被分類為兩大種類：讀取DLL生成物件、讀取Config生成物件。

- 讀取DLL生成物件這個種類，代表性的Framework為MEF，MEF可以透過讀取DLL的資訊來反射生成物件，優點是不需要去維護日益肥大的Config檔，缺點是需要額外設計才能為物件生成提供參數。

- 讀取Config生成物件這個種類，代表性的Framework為Unity，Unity可以透過讀取Config的資訊來反射生成物件，優點是能夠簡單的為物件生成提供參數，缺點是需要去維護日益肥大的Config檔。

這兩大種類的DI Framework，各自的優缺點很鮮明，並且各有不少的擁護者。但是以「簡單」這個角度去思考，會發現讀取DLL生成物件這個種類，需要的背景知識較多且深；而讀取Config生成物件這個種類，需要的背景知識相對單純，所以解決方案的DI模組(DI Framework)設計方向，選擇讀取Config生成物件這個種類來做為設計方向。

###Config結構###

選擇讀取Config生成物件這個種類來做為DI模組的設計方向之後，就需要開始思考Config的結構設計。

- 從.NET中提供反射生成的機制去分析，可以發現在.NET中只要透過「型別的組件限定名稱([AssemblyQualifiedName](http://msdn.microsoft.com/library/system.type.assemblyqualifiedname.ASPX))」，就可以反射生成型別(Type)；然後再使用[Activator.CreateInstance](http://msdn.microsoft.com/library/system.activator.createinstance.aspx)這個方法，就可以反射生成出對應的實體(Entity)。

	以反射生成機制基礎概念、搭配XML概念，可以設計出下列的Config結構：每個反射生成的實體，在Config結構裡使用一個標籤來定義；並且這個實體標籤的屬性裡，需要存放類別的AssemblyQualifiedName，來做為反射生成的必要參數。

		<entity type="NamespaceName.ClassName, AssemblyName" />

- 每個被反射生成的實體(Entity)，或多或少會需要定義一些各自的參數(Parameter)，例如說：反射生成一個SqlRepository類別,就會需要一個資料庫連線字串的參數。為了這個使用情景，所以為Config結構加入參數的概念：代表實體的標籤，包含一個到多個的參數屬性。

		<entity type="NamespaceName.ClassName, AssemblyName" ParameterA="AAA" ParameterB="BBB" />

- 在某些系統中，不單只需要反射生成一個實體來使用，而是需要生成一組實體集合，例如說：資料彙整的系統，就需要取得所有提供資料的Repository物件集合來使用。為了滿足這個使用情景，所以需要為Config結構加入群組(Group)的概念：Config中使用一個標籤用來定義群組，這個群組標籤裡包含了許多可以被反射生成的實體標籤，同群組標籤內的實體標籤能夠被反射生成為一組實體集合。

		<group>
		  <add type="NamespaceName.ClassName001, AssemblyName" ParameterA="AAA" ParameterB="BBB" />
		  <add type="NamespaceName.ClassName002, AssemblyName" ParameterC="CCC" />
		  <add type="NamespaceName.ClassName003, AssemblyName" ParameterD="DDD" />
		</group>

- 加入了群組的概念之後再回頭檢視，會發現目前的Config結構,因為從Config結構無法得知該生成哪個實體，所以無法滿足反射生成一個實體的使用情景。這時可以為群組裡的每個實體標籤加上實體名稱參數(Name)用來識別每個實體，並且在群組標籤中加入預設實體名稱參數(Default)，當遇到反射生成一個實體的情景，對照群組中的預設實體名稱，就可以反射生成實體名稱對應的實體；而需要生成一組實體集合的情景，在目前的Config結構設計中，依然可以正常的運作。

		<group default="XXX">
		  <add name="XXX" type="NamespaceName.ClassName001, AssemblyName" ParameterA="AAA" ParameterB="BBB" />
		  <add name="YYY" type="NamespaceName.ClassName002, AssemblyName" ParameterC="CCC" />
		  <add name="ZZZ" type="NamespaceName.ClassName003, AssemblyName" ParameterD="DDD" />
		</group>

- 在更複雜的系統中，不單只需要反射生成一種實體或實體集合，而是需要反射生成多種不同的實體或實體集合，例如：資料轉檔的系統，就需要取得輸入資料的Repository、取得輸出資料的Repository同時來使用。為了滿足這個使用情景，所以Config結構需要能夠包含多個群組標籤，並且每個群組標籤需要加上群組名稱，用來識別每個群組。後續對照群組中的群組名稱，就可以反射生成群組名稱對應的實體或實體集合。

		<GroupName001 default="XXX">
		  <add name="XXX" type="NamespaceName.ClassName001, AssemblyName" ParameterA="AAA" ParameterB="BBB" />
		  <add name="YYY" type="NamespaceName.ClassName002, AssemblyName" ParameterC="CCC" />
		  <add name="ZZZ" type="NamespaceName.ClassName003, AssemblyName" ParameterD="DDD" />
		</GroupName001>
        
        <GroupName002>
		  <add name="VVV" type="NamespaceName.ClassName004, AssemblyName" ParameterC="CCC" />
		  <add name="WWW" type="NamespaceName.ClassName005, AssemblyName" ParameterD="DDD" />
		</GroupName002>

###領域模型###

設計完Config結構之後，就可以依照這個結構，來分析DI模組的領域模型。([Eric Evans - 領域驅動設計(DDD)](http://www.books.com.tw/products/CN10710357))

- DI模組包含許多群組(Group)。

	![領域模型01](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E9%A0%98%E5%9F%9F%E6%A8%A1%E5%9E%8B01.png)

- 每個群組包含許多實體(Entity)。
 
	![領域模型02](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E9%A0%98%E5%9F%9F%E6%A8%A1%E5%9E%8B02.png)

- 每個實體包含許多參數(Parameter)。

	![領域模型03](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E9%A0%98%E5%9F%9F%E6%A8%A1%E5%9E%8B03.png)

- 領域模型分析到這邊，需要加入一個額外設計考量：DI模組生成的實體(Entity)，是否應該設計為POCO([Martin Fowler - POJO](http://www.martinfowler.com/bliki/POJO.html))。將實體設計為POCO的優點，是讓實體不相依於DI模組；而缺點則是需要複雜的反射生成機制來取得建構子、注入建構參數，最終才能生成實體物件。

 	很顯然的，DI模組生成的實體不應該相依於DI模組。但是為了不引入複雜的反射生成機制，在這個階段需要為DI模組加入建構者(Builder)概念：DI模組不直接反射生成實體(Entity)，而是反射生成建構者(Builder)，再由建構者去剖析參數(Parameter)，呼叫建構子來建立實體。加入建構者(Builder)概念後，就可以免除在DI模組中引入複雜的反射生成機制，而依然可以生成設計為POCO的實體物件。

	![領域模型04](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E9%A0%98%E5%9F%9F%E6%A8%A1%E5%9E%8B04.png)

- 在領域模型中加入建構者(Builder)概念之後，需要回過頭去設計Config結構，為Config結構加入建構者(Builder)的概念：將群組標籤內的代表實體的標籤，更改設計為代表建構者的標籤。群組標籤內的建構者標籤能夠被反射生成為建構者，透過建構者去剖析參數、呼叫建構子，就可以生成實體物件。

		<GroupName001 default="XXX">
		  <add name="XXX" type="NamespaceName.ClassBuilderName001, AssemblyName" ParameterA="AAA" ParameterB="BBB" />
		  <add name="YYY" type="NamespaceName.ClassBuilderName002, AssemblyName" ParameterC="CCC" />
		  <add name="ZZZ" type="NamespaceName.ClassBuilderName003, AssemblyName" ParameterD="DDD" />
		</GroupName001>
        
        <GroupName002>
		  <add name="VVV" type="NamespaceName.ClassBuilderName004, AssemblyName" ParameterC="CCC" />
		  <add name="WWW" type="NamespaceName.ClassBuilderName005, AssemblyName" ParameterD="DDD" />
		</GroupName002>


###物件模型###

設計完領域模型之後，就可以依照這個模型，來設計DI模組的物件模型。

- 首先將領域模型，套用DDD設計中的Entity模式，來決定進出系統邊界的物件：Group類別、Builder類別。

    ![物件模型01](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E6%A8%A1%E5%9E%8B01.png)

- 接著套用DDD設計中的Aggregate模式，來處理Builder類別與Parameter類別之間的關聯，將Builder類別設計為Parameter類別的聚合根。

	![物件模型02](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E6%A8%A1%E5%9E%8B02.png)

- 再來為進出系統邊界的物件，加入套用DDD設計中的Repository模式，用來定義出系統邊界物件、以及隔離系統與DAL層的相依。在這其中IGroupRepository介面封裝Group類別進出系統邊界的職責、IBuilderRepository介面封裝Builder類別進出系統邊界的職責。	

    ![物件模型03](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E6%A8%A1%E5%9E%8B03.png)

- 而Group類別與Builder類別之間的關聯，在設計出IBuilderRepository介面之後，就可以設計為Group類別提供IBuilderRepository介面，來操作Group類別所包含的Builder類別。

    ![物件模型04](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E6%A8%A1%E5%9E%8B04.png)

- 為了抽換邊界物件的便利性，將系統中IGroupRepository、IBuilderRepository這兩個邊界物件套用設計模式的Facade模式，來整合兩個邊界物件成為一個IRepository。並且套用設計模式中的Adapter模式，來實作GroupRepository、BuilderRepository用以提供系統內部使用。

	![物件模型05](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E6%A8%A1%E5%9E%8B05.png)

- 最後讓使用DI模組的開發人員能夠更簡單的使用，為物件模型套用DDD設計中的Service模式，將各種物件交互運作來反射生成實體的這個職責，封裝成為一個Context類別提供開發人員使用。而在設計這個Context類別的同時，也套用了設計模式的Facade模式，讓Context類別成為DI模組的窗口，用來提供開發人員操作DI模組內的各種物件。  

    ![物件模型06](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E6%A8%A1%E5%9E%8B06.png)


##模組設計##

CLK.Reflection是一個極簡風格的依賴注入模組(DI Framework)。在開發需要依賴注入功能的功能模組時，使用CLK.Reflection能夠幫助開發人員，簡化依賴注入功能的開發工作。

###模組下載###

下載程式碼：[由此進入GitHub後，點選右下角的「Download ZIP」按鈕下載。](https://github.com/Clark159/CLK)

模組程式碼：\CLK方案\01 Domains資料夾\CLK專案\Reflection資料夾\

範例程式碼：\CLK方案\02 Samples資料夾\03 CLK.Reflection.Samples資料夾\

(開啟程式碼的時候，建議使用Visual Studio所提供的「大綱->摺疊至定義」功能來摺疊程式碼，以能得到較適合閱讀的排版樣式。)

###物件結構###

![物件結構01](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E7%B5%90%E6%A7%8B01.png)

- ReflectContext

	- CLK.Reflection模組的根節點物件。		
	
	- 提供CreateEntity方法，用以反射生成實體物件。
	
	- 提供CreateAllEntity方法，用以反射生成實體集合。
	
    - 提供Locator功能(選用)。

- IReflectRepository

 	- 套用Repository模式、Facade模式產生的邊界介面。
 	
 	- 用來隔離系統與DAL之間的相依性。 
 	
 	- 提供設定資料進出模組邊界的CRUD功能。 	

- ReflectBuilder

	- 進出系統邊界的物件單位，使用Name做為索引。
	
	- 使用Dictionary<string, string>結構來封裝Parameter集合。
	
	- 實際被反射生成的物件。
	
	- 提供CreateEntity函式，調用CreateEntity函式之後開始剖析參數內容、呼叫建構子、最終生成實體物件回傳。
	
- ReflectGroup

	- 進出系統邊界的物件單位，使用Name做為索引。
	
	- 提供ReflectBuilderRepository屬性，用以提供ReflectBuilder物件。
	
	- 封裝預設實體名稱，用以反射生成預設實體物件。 

- ReflectGroupRepository

	- 套用Adapter模式產生的邊界物件。
	
	- 轉接IReflectRepository介面的CRUD功能，提供ReflectGroup物件進出模組邊界的CRUD功能。
	
- ReflectBuilderRepository

	- 套用Adapter模式產生的邊界物件。
	
	- 轉接IReflectRepository介面的CRUD功能，提供ReflectBuilder物件進出模組邊界的CRUD功能。
	
	- 轉接IReflectRepository介面的CRUD功能過程中，會將ReflectBuilder物件轉換為對應的DTO物件：ReflectSetting物件來進出系統邊界，用以簡化IReflectRepository介面的實作設計。

- ReflectGroupDictionary

	- 將ReflectGroupRepository物件所提供的CRUD功能，轉接為簡單使用的Dictionary物件樣式提供開發人員使用。
		
- ReflectBuilderDictionary

	- 將ReflectBuilderRepository物件所提供的CRUD功能，轉接為簡單使用的Dictionary物件樣式提供開發人員使用。

- ConfigReflectRepository

	- 繼承IReflectRepository介面。	
	
	- 轉接Config檔的設定內容，用以提供系統來存取Config檔中的反射生成設定。
    
![物件結構02](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E7%B5%90%E6%A7%8B02.png)

- ReflectContext

	- 額外封裝SettingContext物件，用以提供ReflectBuilder類別反射生成實體物件時使用。
	
	- 額外繼承IReflectContext物件，用以提供ReflectBuilder類別反射生成實體物件時使用。

- SettingContext

	- CLK.Settings模組的根節點物件。	

	- 提供參數存取功能。
		
- IReflectContext
	
	- 封裝ReflectContext類別的CreateEntity方法、CreateAllEntity方法給ReflectBuilder類別使用。
	
	- 用以避免ReflectContext類別、ReflectBuilder類別之間的循環相依。


###物件互動###

- 生成預設實體物件

	![物件互動01](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9501.png)

- 生成指定實體物件

	![物件互動02](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9502.png)

- 生成實體物件集合

	![物件互動03](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9503.png)

##使用範例##

###CLK.Reflection.Samples - 建立TestEntity類別、TestEntityBuilder類別

為了降低範例的複雜度，部分範例使用CLK.Reflection.Samples專案中的類別，來做為依賴注入功能的範例物件：

- TestEntity類別：做為依賴注入的實體物件。在TestEntity類別中提供了Print函式，列印參數內容到Console，用以在範例中提供開發人員觀察TestEntity物件內容。
		
		namespace CLK.Reflection.Samples
		{
		    public class TestEntity
		    {
		        // Fields        
		        private readonly string _parameterA = string.Empty;
		
		
		        // Constructors
		        public TestEntity(string parameterA)
		        {
		            #region Contracts
		
		            if (string.IsNullOrEmpty(parameterA) == true) throw new ArgumentNullException();
		
		            #endregion
		
		            // Arguments
		            _parameterA = parameterA;
		        }
		        
		
		        // Methods
		        public void Print()
		        {
		            // Write
		            Console.WriteLine(_parameterA);
		        }
		    }
		}

- TestEntityBuilder類別：做為TestEntity類別的建構者。用以剖析參數內容、呼叫建構子、最終生成TestEntity物件。

		namespace CLK.Reflection.Samples
		{
		    public sealed class TestEntityBuilder : ReflectBuilder
		    {
		        // Properties   
		        public string ParameterA
		        {
		            get { return this.GetParameter("ParameterA"); }
		            set { this.SetParameter("ParameterA", value); }
		        }
		
		
		        // Methods          
		        protected override object CreateEntity()
		        {
		            // Parameters
		            string parameterA = this.ParameterA;
		            if (string.IsNullOrEmpty(parameterA) == true) throw new InvalidOperationException();
		
		            // Create
		            TestEntity testEntity = new TestEntity(parameterA);
		
		            // Return
		            return testEntity;
		        }
		    }
		}

###CLK.Reflection.Samples.No001 - 建立模組###

在使用ReflectContext物件之前，必須先取得系統所使用的ReflectContext物件，在範例中統一透過生成函式來提供ReflectContext物件。例如下列範例中的生成函式，會建立一個ReflectContext物件的子類別：ConfigReflectContext物件，這個ConfigReflectContext物件會讀取Config檔中的相關設定內容，用以提供系統使用DI模組的反射生成功能來完成依賴注入的開發工作。

- 建立模組    

        static ReflectContext Create()
        {
            // ReflectContext
            ReflectContext reflectContext = new ConfigReflectContext();

            // Return
            return reflectContext;
        }

###CLK.Reflection.Samples.No002 - 生成預設實體物件###

- 設定檔

		<?xml version="1.0" encoding="utf-8" ?>
		<configuration>
		
		  <!-- ConfigSections -->
		  <configSections>
		    <section name="TestEntityGroup" type="CLK.Configuration.Reflection.ReflectConfigurationSection, CLK.Configuration" />
		  </configSections>
		
		  <!--TestEntityGroup-->
		  <TestEntityGroup default="XXX">
		    <add name="XXX" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="AAA" />
		    <add name="YYY" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="BBB" />
		    <add name="ZZZ" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="CCC" />
		  </TestEntityGroup>
		
		</configuration>

- 生成預設實體物件

		static void Main(string[] args)
        {
            // Create
            ReflectContext reflectContext = Program.Create();

            // CreateEntity
            TestEntity testEntity = reflectContext.CreateEntity<TestEntity>("TestEntityGroup");            

            // Print
            Console.WriteLine("\nTestEntity.Print()");
            testEntity.Print();

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例02](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B02.png)

###CLK.Reflection.Samples.No003 - 生成指定實體物件###

- 設定檔

		<?xml version="1.0" encoding="utf-8" ?>
		<configuration>
		
		  <!-- ConfigSections -->
		  <configSections>
		    <section name="TestEntityGroup" type="CLK.Configuration.Reflection.ReflectConfigurationSection, CLK.Configuration" />
		  </configSections>
		
		  <!--TestEntityGroup-->
		  <TestEntityGroup default="XXX">
		    <add name="XXX" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="AAA" />
		    <add name="YYY" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="BBB" />
		    <add name="ZZZ" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="CCC" />
		  </TestEntityGroup>
		
		</configuration>

- 生成指定實體物件

		static void Main(string[] args)
        {
            // Create
            ReflectContext reflectContext = Program.Create();

            // CreateEntity
            TestEntity testEntity = reflectContext.CreateEntity<TestEntity>("TestEntityGroup", "YYY");            

            // Print
            Console.WriteLine("\nTestEntity.Print()");
            testEntity.Print();

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例03](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B03.png)

###CLK.Reflection.Samples.No004 - 生成實體物件集合###

- 設定檔

		<?xml version="1.0" encoding="utf-8" ?>
		<configuration>
		
		  <!-- ConfigSections -->
		  <configSections>
		    <section name="TestEntityGroup" type="CLK.Configuration.Reflection.ReflectConfigurationSection, CLK.Configuration" />
		  </configSections>
		
		  <!--TestEntityGroup-->
		  <TestEntityGroup default="XXX">
		    <add name="XXX" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="AAA" />
		    <add name="YYY" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="BBB" />
		    <add name="ZZZ" type="CLK.Reflection.Samples.TestEntityBuilder, CLK.Reflection.Samples" ParameterA="CCC" />
		  </TestEntityGroup>
		
		</configuration>

- 生成實體物件集合

		static void Main(string[] args)
        {
            // Create
            ReflectContext reflectContext = Program.Create();

            // CreateAllEntity
            IEnumerable<TestEntity> testEntityCollection = reflectContext.CreateAllEntity<TestEntity>("TestEntityGroup");            

            // Print
            Console.WriteLine("\nTestEntity.Print()");
            foreach (var testEntity in testEntityCollection)
            {
                testEntity.Print();
            }

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例04](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B04.png)

###CLK.Reflection.Samples.No005 - 讀取連線字串###

操作資料庫功能的類別再被反射生成的時候，必須提供資料庫的連線字串給物件所使用，而資料庫的連線字串通常會建議存放在config檔的connectionStrings區段裡。為了符合這個使用情景，在CLK.Reflection模組中，使用CLK.Settings模組做為參數存取模組，來提供connectionStrings、AppSetting等等相關參數的存取功能。

- SqlRepository類別：做為依賴注入的實體物件。在SqlRepository類別中提供了ConnectionString屬性，用以在範例中提供開發人員觀察SqlRepository物件內容。

		namespace CLK.Reflection.Samples.No005
		{
		    public class SqlRepository
		    {
		        // Constructors
		        public SqlRepository(string connectionString)
		        {
		            #region Contracts
		
		            if (string.IsNullOrEmpty(connectionString) == true) throw new ArgumentNullException();
		
		            #endregion
		
		            // Arguments
		            this.ConnectionString = connectionString;
		        }
		
		
		        // Properties   
		        public string ConnectionString { get; private set; }
		    }
		}


- SqlRepositoryBuilder類別：做為SqlRepository類別的建構者。用以剖析參數內容、讀取連線字串、呼叫建構子、最終生成SqlRepository物件。

		namespace CLK.Reflection.Samples.No005
		{
		    public sealed class SqlRepositoryBuilder : ReflectBuilder
		    {
		        // Properties   
		        public string ConnectionStringName
		        {
		            get { return this.GetParameter("ConnectionStringName"); }
		            set { this.SetParameter("ConnectionStringName", value); }
		        }
		
		
		        // Methods          
		        protected override object CreateEntity()
		        {
		            // ConnectionStringName
		            string connectionStringName = this.ConnectionStringName;
		            if (string.IsNullOrEmpty(connectionStringName) == true) throw new InvalidOperationException();
		
		            // ConnectionString
		            string connectionString = this.SettingContext.ConnectionStrings[connectionStringName];
		            if (string.IsNullOrEmpty(connectionString) == true) throw new InvalidOperationException();
		
		            // Create
		            SqlRepository sqlRepository = new SqlRepository(connectionString);
		
		            // Return
		            return sqlRepository;
		        }
		    }
		}

- 設定檔

		<?xml version="1.0" encoding="utf-8" ?>
		<configuration>
		
		  <!-- ConfigSections -->
		  <configSections>
		    <section name="SqlRepositoryGroup" type="CLK.Configuration.Reflection.ReflectConfigurationSection, CLK.Configuration" />
		  </configSections>
		
		  <!--connectionStrings-->
		  <connectionStrings>
		    <add name="MainDB"  connectionString="Data Source=127.1.1.1;Initial Catalog=DB001;Uid=sa;Pwd=1234;Persist Security Info=True" />
		    <add name="OtherDB" connectionString="Data Source=127.2.2.2;Initial Catalog=DB002;Uid=sa;Pwd=1234;Persist Security Info=True" />
		  </connectionStrings>
		
		  <!--SqlRepositoryGroup-->
		  <SqlRepositoryGroup default="XXX">
		    <add name="XXX" type="CLK.Reflection.Samples.No005.SqlRepositoryBuilder, CLK.Reflection.Samples.No005" ConnectionStringName="MainDB" />
		    <add name="YYY" type="CLK.Reflection.Samples.No005.SqlRepositoryBuilder, CLK.Reflection.Samples.No005" ConnectionStringName="OtherDB" />
		    <add name="ZZZ" type="CLK.Reflection.Samples.No005.SqlRepositoryBuilder, CLK.Reflection.Samples.No005" ConnectionStringName="OtherDB" />
		  </SqlRepositoryGroup>
		
		</configuration>

- 讀取連線字串
		
		static void Main(string[] args)
        {
            // Create
            ReflectContext reflectContext = Program.Create();

            // CreateEntity
            SqlRepository sqlRepository = reflectContext.CreateEntity<SqlRepository>("SqlRepositoryGroup");

            // Print
            Console.WriteLine("\nTestEntity.ConnectionString");
            Console.WriteLine(sqlRepository.ConnectionString);

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例05](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B05.png)


###CLK.Reflection.Samples.No006 - 生成巢狀物件###

當生成Top物件的時候，需要另外一個Sub物件做為參數來建構，這是物件導向中很常見的物件設計。為了生成這類的巢狀物件，CLK.Reflection模組支援TopBuilder類別在生成Top物件的同時，使用群組名稱去反射生成Sub物件，來做為Top物件的建構參數。

- TopEntity類別：做為依賴注入的實體物件。生成TopEntity類別需要在建構子中提供SubEntity物件做為建構參數，並且這個SubEntity物件會做為TopEntity類別的屬性，用以在範例中提供開發人員觀察TopEntity物件中SubEntity物件的內容。

		namespace CLK.Reflection.Samples.No006
		{
		    public class TopEntity
		    {
		        // Constructors
		        public TopEntity(SubEntity subEntity)
		        {
		            #region Contracts
		
		            if (subEntity == null) throw new ArgumentNullException();
		
		            #endregion
		
		            // Arguments
		            this.SubEntity = subEntity;
		        }
		
		
		        // Properties   
		        public SubEntity SubEntity { get; private set; }
		    }
		}

- TopEntityBuilder類別：做為TopEntity類別的建構者。用以剖析參數內容、生成SubEntity物件、呼叫建構子、最終生成TopEntity物件。

		namespace CLK.Reflection.Samples.No006
		{
		    public sealed class TopEntityBuilder : ReflectBuilder
		    {
		        // Properties   
		        public string SubEntityGroupName
		        {
		            get { return this.GetParameter("SubEntityGroupName"); }
		            set { this.SetParameter("SubEntityGroupName", value); }
		        }
		
		
		        // Methods          
		        protected override object CreateEntity()
		        {
		            // SubEntityGroupName
		            string subEntityGroupName = this.SubEntityGroupName;
		            if (string.IsNullOrEmpty(subEntityGroupName) == true) throw new InvalidOperationException();
		
		            // SubEntity
		            SubEntity subEntity = this.ReflectContext.CreateEntity<SubEntity>(subEntityGroupName);
		            if (subEntity == null) throw new InvalidOperationException();
		
		            // Create
		            TopEntity topEntity = new TopEntity(subEntity);
		
		            // Return
		            return topEntity;
		        }
		    }
		}

- SubEntity類別：做為依賴注入的實體物件的建構參數。在SubEntity類別中提供了Print函式，列印參數內容到Console，用以在範例中提供開發人員觀察SubEntity物件內容。

		namespace CLK.Reflection.Samples.No006
		{
		    public class SubEntity
		    {
		        // Fields        
		        private readonly string _parameterA = string.Empty;
		
		
		        // Constructors
		        public SubEntity(string parameterA)
		        {
		            #region Contracts
		
		            if (string.IsNullOrEmpty(parameterA) == true) throw new ArgumentNullException();
		
		            #endregion
		
		            // Arguments
		            _parameterA = parameterA;
		        }
		
		
		        // Methods
		        public void Print()
		        {
		            // Write
		            Console.WriteLine(_parameterA);
		        }
		    }
		}

- SubEntityBuilder類別：做為SubEntity類別的建構者。用以剖析參數內容、呼叫建構子、最終生成SubEntity物件。

		namespace CLK.Reflection.Samples.No006
		{
		    public sealed class SubEntityBuilder : ReflectBuilder
		    {
		        // Properties   
		        public string ParameterA
		        {
		            get { return this.GetParameter("ParameterA"); }
		            set { this.SetParameter("ParameterA", value); }
		        }
		
		
		        // Methods          
		        protected override object CreateEntity()
		        {
		            // Parameters
		            string parameterA = this.ParameterA;
		            if (string.IsNullOrEmpty(parameterA) == true) throw new InvalidOperationException();
		
		            // Create
		            SubEntity subEntity = new SubEntity(parameterA);
		
		            // Return
		            return subEntity;
		        }
		    }
		}

- 設定檔

		<?xml version="1.0" encoding="utf-8" ?>
		<configuration>
		
		  <!-- ConfigSections -->
		  <configSections>
		    <section name="TopEntityGroup" type="CLK.Configuration.Reflection.ReflectConfigurationSection, CLK.Configuration" />
		    <section name="SubEntityGroup" type="CLK.Configuration.Reflection.ReflectConfigurationSection, CLK.Configuration" />
		  </configSections>
		
		  <!--TopEntityGroup-->
		  <TopEntityGroup default="Top001">
		    <add name="Top001" type="CLK.Reflection.Samples.No006.TopEntityBuilder, CLK.Reflection.Samples.No006" SubEntityGroupName="SubEntityGroup" />
		    <add name="Top002" type="CLK.Reflection.Samples.No006.TopEntityBuilder, CLK.Reflection.Samples.No006" SubEntityGroupName="SubEntityGroup" />
		    <add name="Top003" type="CLK.Reflection.Samples.No006.TopEntityBuilder, CLK.Reflection.Samples.No006" SubEntityGroupName="SubEntityGroup" />
		  </TopEntityGroup>
		
		  <!--SubEntityGroup-->
		  <SubEntityGroup default="Sub002">
		    <add name="Sub001" type="CLK.Reflection.Samples.No006.SubEntityBuilder, CLK.Reflection.Samples.No006" ParameterA="AAA" />
		    <add name="Sub002" type="CLK.Reflection.Samples.No006.SubEntityBuilder, CLK.Reflection.Samples.No006" ParameterA="BBB" />
		  </SubEntityGroup>
		
		</configuration>

- 生成巢狀物件

		static void Main(string[] args)
        {
            // Create
            ReflectContext reflectContext = Program.Create();

            // CreateEntity
            TopEntity topEntity = reflectContext.CreateEntity<TopEntity>("TopEntityGroup");

            // Print
            Console.WriteLine("\nTopEntity.SubEntity.Print()");
            topEntity.SubEntity.Print();

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例06](https://raw.github.com/Clark159/CLK/master/doc/CLK.Reflection/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B06.png)
