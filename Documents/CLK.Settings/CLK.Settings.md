#[CLK Framework] CLK.Settings - 跨平台的參數存取模組#


##問題情景##

開發功能模組的時候，常常免不了有一些參數(例如ConnectionString)，需要存放在Config檔(App.Config、Web.Config)。而.NET Framework也很貼心的提供System.Configuration命名空間裡的類別，用來幫助開發人員簡化存取Config檔的開發工作。但是當功能模組的開發，是以跨平台執行為目標來做設計的時候，因為不是每個平台都允許Config檔的存在，所以連帶的System.Configuration命名空間裡的類別，也並不支援跨平台的參考使用。

像是開啟一個可攜式類別庫專案(Portable Class Library)，來設計橫跨ASP.NET、Windows Phone兩個平台的功能模組時，因為Windows Phone平台不支援Config檔的存在，所以開發時無法在專案中加入System.Configuration命名空間的參考，用來存取Config檔內的參數設定。

遇到這樣跨平台的功能模組開發，該如何處理跨平台的參數存取呢?

![問題情景01](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E5%95%8F%E9%A1%8C%E6%83%85%E6%99%AF01.png)


##解決方案##

首先，不是每個平台都允許Config檔的存在，每個平台各自擁有平台自己的參數存取功能，像是Windows Phone平台不允許的Config檔的存在，但是可以透過操作IsolatedStorage來提供參數存取功能。為了切割與這些平台參數存取功能的相依性，可以套用IoC模式在功能模組上，用以建立一層參數存取介面，讓跨平台功能模組單純相依於同模組內的參數存取介面。接著再套用Adapter模式建立轉接物件，來將每個平台的參數存取功能，轉接成為參數存取介面的實作。這樣後續就可以依照執行平台的不同，為跨平台功能模組注入不同的參數存取介面實作，用以提供不同平台上參數存取的功能。

![解決方案01](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E8%A7%A3%E6%B1%BA%E6%96%B9%E6%A1%8801.png)

假設，開發一個跨平台功能模組，就要對應開發一個套用Adapter模式的轉接物件，來將每個平台的參數存取功能轉接成為參數存取介面的實作。這樣的開發流程在遇到跨平台功能模組越來越多的情況下，會發現需要開發的轉接物件數量會迅速膨脹，進而成為開發人員工作上的負擔。

![解決方案02](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E8%A7%A3%E6%B1%BA%E6%96%B9%E6%A1%8802.png)

為了簡化轉接物件的開發工作，可以將參數存取功能抽象化之後，建立成為一個共用的參數存取模組，並且這個共用參數存取模組同樣透過套用IoC模式，來切割與平台參數存取功能之間的相依性，讓這個共用參數存取模組可以跨平台使用。後續只需要為每個平台專有的參數存取功能，開發一個轉接進共用參數存取模組的轉接物件，就能讓每個使用共用參數存取模組的跨平台功能模組，能夠透過平台的參數存取功能來存取參數設定。

![解決方案03](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E8%A7%A3%E6%B1%BA%E6%96%B9%E6%A1%8803.png)


##模組設計##

CLK.Settings是一個跨平台的參數存取模組。在開發跨平台功能模組時，使用CLK.Settings能夠幫助開發人員，簡化參數存取功能的開發工作。

###模組下載###

下載程式碼：[由此進入GitHub後，點選右下角的「Download ZIP」按鈕下載。](https://github.com/Clark159/CLK)

模組程式碼：\CLK方案\99 Libraries資料夾\CLK專案\Settings資料夾\

範例程式碼：\CLK方案\01 Samples\01 CLK.Settings.Samples資料夾\

(開啟程式碼的時候，建議使用Visual Studio所提供的「大綱->摺疊至定義」功能來摺疊程式碼，以能得到較適合閱讀的排版樣式。)

###物件結構###

![物件結構01](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E7%89%A9%E4%BB%B6%E7%B5%90%E6%A7%8B01.png)

- SettingContext

	- CLK.Settings模組的根節點物件。	
	
    - 提供Locator功能(選用)。

- SettingDictionary

	- 將ISettingRepository介面所提供的CRUD功能，轉接為簡單使用的Dictionary物件樣式提供開發人員使用。	
	
	- 繼承自CLK.Collections.StoreDictionary

- ISettingRepository

	- 套用IoC模式產生的邊界介面。	
	
	- 用來隔離系統與不同平台參數存取功能之間的相依性。
	
	- 提供參數資料進出模組邊界的CRUD功能。 
	
	- 繼承自CLK.Collections.IStoreProvider

- ConfigConnectionStringRepository

	- 繼承ISettingRepository介面。	
	
	- 轉接Config檔的連線字串存取功能，用以提供系統來存取Config檔中的連線字串設定。

- ConfigAppSettingRepository

	- 繼承ISettingRepository介面。	
	
	- 轉接Config檔的AppSetting存取功能，用以提供系統來存取Config檔中的AppSetting設定。

- MemorySettingRepository

	- 繼承ISettingRepository介面。	
	
	- 在記憶體中建立一個參數集合物件來存放參數設定，用以提供系統來存取記憶體中的參數設定。


###物件互動###

- 讀取參數

	![物件互動01](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9501.png)

- 列舉參數

	![物件互動02](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9502.png)

- 新增參數

	![物件互動03](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9503.png)

- 移除參數

	![物件互動04](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E7%89%A9%E4%BB%B6%E4%BA%92%E5%8B%9504.png)


##使用範例##

###CLK.Settings.Samples.No001 - 建立模組###

在使用SettingContext物件之前，必須先取得系統所使用的SettingContext物件，在範例中統一透過生成函式來提供SettingContext物件。例如下列範例中的生成函式，會建立一個SettingContext物件的子類別：ConfigSettingContext物件，這個ConfigSettingContext物件會轉接Config檔的參數存取功能，用以提供系統來存取Config檔中的參數設定。

- 建立模組    

		static SettingContext Create()
        {
            // SettingContext
            SettingContext settingContext = new ConfigSettingContext();

            // Return
            return settingContext;
        }

###CLK.Settings.Samples.No002 - 讀取參數###

- 設定檔

		<!--AppSettings-->
		<appSettings>
		  <add key="Argument01" value="AAAAAAAAAAAAA" />
		  <add key="Argument02" value="BBBBBBBBBBBBB" />
		  <add key="Argument03" value="CCCCCCCCCCCCC" />
		</appSettings>
		
		<!--ConnectionStrings-->
		<connectionStrings>
		  <clear/>
		  <add name="Database01" connectionString="Data Source=192.168.1.1;Initial Catalog=DB01;" />    
		  <add name="Database02" connectionString="Data Source=192.168.2.2;Initial Catalog=DB02;" />
		</connectionStrings>  

- 讀取參數

        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = Program.Create();

            // Get
            Console.WriteLine("\nAppSettings");
            string argumentString = settingContext.AppSettings["Argument01"];
            Console.WriteLine(argumentString);

            Console.WriteLine("\nConnectionStrings");
            string connectionString = settingContext.ConnectionStrings["Database01"];
            Console.WriteLine(connectionString);
            
            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例02](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B02.png)

###CLK.Settings.Samples.No003 - 列舉參數###

- 設定檔

		<!--AppSettings-->
		<appSettings>
		  <add key="Argument01" value="AAAAAAAAAAAAA" />
		  <add key="Argument02" value="BBBBBBBBBBBBB" />
		  <add key="Argument03" value="CCCCCCCCCCCCC" />
		</appSettings>
		
		<!--ConnectionStrings-->
		<connectionStrings>
		  <clear/>
		  <add name="Database01" connectionString="Data Source=192.168.1.1;Initial Catalog=DB01;" />    
		  <add name="Database02" connectionString="Data Source=192.168.2.2;Initial Catalog=DB02;" />
		</connectionStrings>  

- 列舉參數

        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = Program.Create();
            
            // List
            Console.WriteLine("\nAppSettings");
            foreach (string key in settingContext.AppSettings.Keys)
            {
                string argumentString = settingContext.AppSettings[key];
                Console.WriteLine(argumentString);
            }     

            Console.WriteLine("\nConnectionStrings");
            foreach (string key in settingContext.ConnectionStrings.Keys)
            {
                string connectionString = settingContext.ConnectionStrings[key];
                Console.WriteLine(connectionString);
            }                                   

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果

	![使用範例03](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B03.png)

###CLK.Settings.Samples.No004 - 新增參數###

- 原始設定檔

		<!--AppSettings-->
		<appSettings>
		  <add key="Argument01" value="AAAAAAAAAAAAA" />
		  <add key="Argument02" value="BBBBBBBBBBBBB" />
		  <add key="Argument03" value="CCCCCCCCCCCCC" />
		</appSettings>
		
		<!--ConnectionStrings-->
		<connectionStrings>
		  <clear/>
		  <add name="Database01" connectionString="Data Source=192.168.1.1;Initial Catalog=DB01;" />    
		  <add name="Database02" connectionString="Data Source=192.168.2.2;Initial Catalog=DB02;" />
		</connectionStrings>  

- 新增參數

        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = Program.Create();

            // Set           
            settingContext.AppSettings.Add("Argument04", "DDDDDDDDDDDDD");

            settingContext.ConnectionStrings.Add("Database03", "Data Source=192.168.3.3;Initial Catalog=DB03");

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 結果設定檔

		<!--AppSettings-->
		<appSettings>
		  <add key="Argument01" value="AAAAAAAAAAAAA" />
		  <add key="Argument02" value="BBBBBBBBBBBBB" />
		  <add key="Argument03" value="CCCCCCCCCCCCC" />
		  <add key="Argument04" value="DDDDDDDDDDDDD" />
		</appSettings>
		
		<!--ConnectionStrings-->
		<connectionStrings>
		  <clear />
		  <add name="Database01" connectionString="Data Source=192.168.1.1;Initial Catalog=DB01;" />
		  <add name="Database02" connectionString="Data Source=192.168.2.2;Initial Catalog=DB02;" />
		  <add name="Database03" connectionString="Data Source=192.168.3.3;Initial Catalog=DB03" />
		</connectionStrings>  

###CLK.Settings.Samples.No005 - 移除參數###

- 原始設定檔

		<!--AppSettings-->
		<appSettings>
		  <add key="Argument01" value="AAAAAAAAAAAAA" />
		  <add key="Argument02" value="BBBBBBBBBBBBB" />
		  <add key="Argument03" value="CCCCCCCCCCCCC" />
		</appSettings>
		
		<!--ConnectionStrings-->
		<connectionStrings>
		  <clear/>
		  <add name="Database01" connectionString="Data Source=192.168.1.1;Initial Catalog=DB01;" />    
		  <add name="Database02" connectionString="Data Source=192.168.2.2;Initial Catalog=DB02;" />
		</connectionStrings>  

- 移除參數

        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = Program.Create();

            // Set           
            settingContext.AppSettings.Remove("Argument03");

            settingContext.ConnectionStrings.Remove("Database02");

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 結果設定檔

		<!--AppSettings-->
		<appSettings>
		  <add key="Argument01" value="AAAAAAAAAAAAA" />
		  <add key="Argument02" value="BBBBBBBBBBBBB" />
		</appSettings>
		
		<!--ConnectionStrings-->
		<connectionStrings>
		  <clear />
		  <add name="Database01" connectionString="Data Source=192.168.1.1;Initial Catalog=DB01;" />
		</connectionStrings>  

###CLK.Settings.Samples.No006 - 更換來源###

只要更換生成函式所提供的SettingContext物件，就可以更換系統所使用的參數設定來源。例如下列範例中的生成函式，改為會建立一個SettingContext物件的子類別：MemorySettingContext物件，這個MemorySettingContext物件，會在記憶體中建立一個參數集合物件來存放參數設定，用以提供系統來存取記憶體中的參數設定。

(額外一提，作單元測試的時候，使用MemorySettingContext物件來取代ConfigSettingContext物件，就可以透過程式碼來提供測試用的參數設定，而不需要另外維護Config檔來提供測試用的參數設定。)

- 更換來源

        static SettingContext Create()
        {
            // AppSettingRepository
            MemorySettingRepository appSettingRepository = new MemorySettingRepository();
            appSettingRepository.Add("Argument05", "XXXXXXXXXXXXX");
            appSettingRepository.Add("Argument06", "YYYYYYYYYYYYY");
            appSettingRepository.Add("Argument07", "ZZZZZZZZZZZZZ");

            // ConnectionStringRepository
            MemorySettingRepository connectionStringRepository = new MemorySettingRepository();
            connectionStringRepository.Add("Database04", "Data Source=192.168.4.4;Initial Catalog=DB04;");
            connectionStringRepository.Add("Database05", "Data Source=192.168.5.5;Initial Catalog=DB05;");  

            // SettingContext
            SettingContext settingContext = new MemorySettingContext(appSettingRepository, connectionStringRepository);

            // Return
            return settingContext;
        }

- 列舉參數

        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = Program.Create();
            
            // List
            Console.WriteLine("\nAppSettings");
            foreach (string key in settingContext.AppSettings.Keys)
            {
                string argumentString = settingContext.AppSettings[key];
                Console.WriteLine(argumentString);
            }     

            Console.WriteLine("\nConnectionStrings");
            foreach (string key in settingContext.ConnectionStrings.Keys)
            {
                string connectionString = settingContext.ConnectionStrings[key];
                Console.WriteLine(connectionString);
            }                                   

            // End
            Console.WriteLine("\nPress enter to end...");
            Console.ReadLine();
        }

- 執行結果
  
	![使用範例06](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Settings/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B06.png)
