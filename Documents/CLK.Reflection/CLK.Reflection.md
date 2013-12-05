#[CLK Framework] CLK.Reflection - 極簡風格的依賴注入模組(DI Framework)#


##問題情景##

IoC模式在近代軟體設計中已經成了顯學。不管是將系統設計為抽換邊界物件、或者是將物件設計為能夠自動測試...等等，都可以看到IoC模式的身影。而在系統內加入IoC模式來隔離系統物件與外部物件的相依時，最終大多會透過各種DI Framework的物件生成功能來完成依賴注入的設計，用以在執行期間決定生成何種外部物件來提供系統使用。

一般業界常見的DI Framework有：MEF、Unity、Spring、Autofac等等，這些DI Framework的功能強大，幾乎能夠滿足所有依賴注入的相關設計。但是，也因為這些DI Framework的功能強大，許多額外的功能也被加到Framework的設計之中，例如：物件獨體生成、物件組合生成、物件生命管理等等，這些額外加入Framework的功能，提供開發人員設計系統時的便利，但是也大大提高了學習的門檻。光是搞懂每個DI Framework的設計細節，就需要消耗掉開發人員大量的腦力與時間。

單純想要使用DI Framework的物件生成功能來完成依賴注入的設計，是否有更簡單的解決方案呢?


##解決方案##

一般業界常見的DI Framework，可以被分類為兩大種類：讀取DLL生成物件、讀取Config生成物件。

- 讀取DLL生成物件這個種類，代表性的Framework為MEF，MEF可以透過讀取DLL的資訊來反射生成物件，優點是不需要去維護日益肥大的Config檔，缺點是需要額外設計才能為物件生成提供參數。

- 讀取Config生成物件這個種類，代表性的Framework為Unity，Unity可以透過讀取Config的資訊來反射生成物件，優點是能夠簡單的為物件生成提供參數，缺點是需要去維護日益肥大的Config檔。

這兩大種類的DI Framework，各自的優缺點很鮮明，並且各有不少的擁護者。但是以「更簡單」這個角度去思考，會發現讀取DLL生成物件這個種類，需要的背景知識較多且深；而讀取Config生成物件這個種類，需要的背景知識相對單純，所以解決方案的設計方向，選擇讀取Config生成物件這個種類來做為設計方向。


###Config結構###

選擇讀取Config生成物件這個種類來做為解決方案的設計方向之後，就需要開始思考Config的結構設計。

- 從.NET中提供反射生成的機制去分析，可以發現在.NET中只要透過「型別的組件限定名稱([AssemblyQualifiedName](http://msdn.microsoft.com/library/system.type.assemblyqualifiedname.ASPX))」，就可以反射生成型別(Type)；然後再使用[Activator.CreateInstance](http://msdn.microsoft.com/library/system.activator.createinstance.aspx)這個方法，就可以反射生成出對應的實體(Entity)。以此為基礎該念、搭配XML概念，可以設計出下列的Config結構：每個可以被反射生成的實體，在Config結構裡使用一個標籤來定義；並且這個實體標籤的屬性裡，需要存放類別的AssemblyQualifiedName，來做為反射生成的必要參數。

		<entity type="NamespaceName.ClassName, AssemblyName" />

- 每個被反射生成的實體(Entity)，或多或少會需要定義一些各自的參數(Parameter)，例如說：反射生成一個SqlRepository類別,就會需要一個資料庫連線字串的參數。為了這個使用情景，所以為Config結構加入參數的概念：每個可以被反射生成的實體標籤，能夠包含一個到多個的參數屬性。

		<entity type="NamespaceName.ClassName, AssemblyName" ParameterA="AAA" ParameterB="BBB" />

- 在某些系統中，不單只需要反射生成一個實體來使用，而是需要生成一組實體集合，例如說：資料彙整的系統，就需要一次取得所有提供資料的Repository物件集合來使用。為了滿足這個使用情景，所以需要為Config結構加入群組(Group)的概念：Config中使用一個標籤用來定義群組，這個群組標籤裡包含了許多可以被反射生成的實體標籤，同群組標籤內的實體標籤能夠被反射生成為一組實體集合。

		<group>
		  <add type="NamespaceName.ClassNameA, AssemblyName" ParameterA="AAA" ParameterB="BBB" />
		  <add type="NamespaceName.ClassNameB, AssemblyName" ParameterC="CCC" />
		  <add type="NamespaceName.ClassNameC, AssemblyName" ParameterD="DDD" />
		</group>

- 加入了群組的概念之後再回頭檢視，會發現目前的Config結構無法滿足，反射生成一個實體這樣的使用情景,因為從Config結構無法得知該生成哪個實體。這時可以為群組裡的每個實體加上實體名稱(Name)參數用來識別每個實體，並且在群組中加入預設實體名稱(Default)參數，當需要反射生成一個實體的情景，對照群組中的預設實體名稱，就可以反射生成實體名稱對應的實體；而需要生成一組實體集合的情景，在目前的Config結構設計中，依然可以正常的運作。

		<group default="XXX">
		  <add name="XXX" type="NamespaceName.ClassNameA, AssemblyName" ParameterA="AAA" ParameterB="BBB" />
		  <add name="YYY" type="NamespaceName.ClassNameB, AssemblyName" ParameterC="CCC" />
		  <add name="ZZZ" type="NamespaceName.ClassNameC, AssemblyName" ParameterD="DDD" />
		</group>


###領域模型###

###物件模型###


##模組設計##

###模組下載###

程式碼下載：[由此進入GitHub後，點選右下角的「Download ZIP」按鈕下載。](https://github.com/Clark159/CLK)

(開啟程式碼的時候，建議使用Visual Studio所提供的「大綱->摺疊至定義」功能來摺疊程式碼，以能得到較適合閱讀的排版樣式。)

###物件結構###

###物件互動###


##使用範例##

###CLK.Settings.Samples.No001 - 建立模組###

###CLK.Settings.Samples.No002 - 讀取參數###