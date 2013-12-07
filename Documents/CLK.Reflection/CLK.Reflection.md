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

設計完Config結構之後，就可以依照這個結構，來分析DI模組的領域模型。

- DI模組包含許多群組(Group)。

		<圖>

- 每個群組包含許多實體(Entity)。
 
		<圖>

- 每個實體包含許多參數(Parameter)。

		<圖>

- 領域模型分析到這邊，需要加入一個額外設計考量：DI模組生成的實體(Entity)，是否應該設計為POCO([Martin Fowler - POJO](http://www.martinfowler.com/bliki/POJO.html))。將實體設計為POCO的優點，是讓實體不相依於DI模組；而缺點則是需要複雜的反射生成機制來取得建構子、注入建構參數，最終才能生成實體物件。

 	很顯然的，DI模組生成的實體不應該相依於DI模組。但是為了不引入複雜的反射生成機制，在這個階段需要為DI模組加入建構者(Builder)概念：DI模組不直接反射生成實體(Entity)，而是反射生成建構者(Builder)，再由建構者去剖析參數(Parameter)，呼叫建構子來建立實體。加入建構者(Builder)概念後，就可以免除在DI模組中引入複雜的反射生成機制，而依然可以生成設計為POCO的實體物件。

    	<圖>

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

- 首先將領域模型，套用DDD設計中的Entity模式、Value Object模式，來決定進出系統邊界的物件單位。

    	<圖>

- 接著為進出系統邊界的物件單位，加入對應的Repository類別，用來定義出系統邊界物件、並且隔離系統與DAL層的相依。

    	<圖>

- 為了讓使用DI模組的開發人員能夠更簡單的使用，為物件模型套用DDD設計中的Service模式，來將需要各種物件交互運作，才能完成反射生成實體的這個職責，封裝成為一個Context類別。而在設計這個Context類別的背後，也套用了設計模式的Facade模式，讓Context類別成為DI模組的窗口，用來提供開發人員操作DI模組內的各種物件。

    	<圖>

- 最後為了抽換邊界物件的便利性，將系統中IGroupRepository、IBuilderRepository這兩個邊界物件套用設計模式的Facade模式，來整合兩個邊界物件成為一個。並且套用設計模式中的Adapter模式，來實作GroupRepository、BuilderRepository用以提供系統內部使用。

		<圖>


##模組設計##

CLK.Reflection是一個極簡風格的依賴注入模組(DI Framework)。在開發需要依賴注入功能的功能模組時，使用CLK.Reflection能夠幫助開發人員，簡化依賴注入功能的開發工作。

###模組下載###

下載程式碼：[由此進入GitHub後，點選右下角的「Download ZIP」按鈕下載。](https://github.com/Clark159/CLK)

模組程式碼：\CLK方案\99 Libraries資料夾\CLK專案\Reflection資料夾\

範例程式碼：\CLK方案\01 Samples\03 CLK.Reflection.Samples資料夾\

(開啟程式碼的時候，建議使用Visual Studio所提供的「大綱->摺疊至定義」功能來摺疊程式碼，以能得到較適合閱讀的排版樣式。)

###物件結構###

###物件互動###


##使用範例##

###CLK.Settings.Samples.No001 - 建立模組###

###CLK.Settings.Samples.No002 - 讀取參數###