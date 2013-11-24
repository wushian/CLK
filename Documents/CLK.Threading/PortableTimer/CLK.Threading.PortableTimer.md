#[CLK Framework] CLK.Threading.PortableTimer - 跨平台的Timer類別#


##問題情景##

開發應用程式的時候，免不了需要加入一些定時執行的設計，例如說：定時更新畫面資料、定時檢查資料庫內容、定時檢查通訊是否斷線...等等。而.NET Framework也很貼心的提供三種不同的Timer類別，用來幫助開發人員設計出定時執行的功能模組。

.NET Framework提供的三種Timer類別，可以參考Bill叔的部落格：

- [三種時間人《.NET中的Timer(1)》](http://www.dotblogs.com.tw/billchung/archive/2009/04/18/8044.aspx)

- [三種時間人《.NET中的Timer(2)》](http://www.dotblogs.com.tw/billchung/archive/2009/04/19/8052.aspx)
 
- [三種時間人《.NET中的Timer(3)》](http://www.dotblogs.com.tw/billchung/archive/2009/05/03/8277.aspx) 

但是當功能模組的開發，是以跨平台執行為目標來做設計的時候，因為不是每個平台都支援上列三種Timer，所以連帶的在跨平台的專案中，也就不支援參考使用.NET Framework所提供的Timer類別。像是下圖中所建立的Portable Class Library專案，就無法參考使用到System.Threading.Timer類別。

遇到這樣跨平台的功能模組開發，該如何提供跨平台的定時執行功能呢?

![問題情景01](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Threading/PortableTimer/Images/%E5%95%8F%E9%A1%8C%E6%83%85%E6%99%AF01.png)


##解決方案##

處理跨平台的定時執行功能，其實解決方案很簡單，只要建立一個跨平台的Timer類別，用來提供定時執行的功能，就可以滿足這個設計需求。


##模組設計##

建立Timer類別最簡單的設計，就是開啟一條獨立的執行緒，透過這個執行緒定時去執行Callback函式，這就完成了Timer類別的功能設計。但是因為.NET Framework中所提供的System.Threading.Thread並不支援跨平台使用。所以執行緒的建立工作，必須改為可以跨平台使用的System.Threading.Tasks.Task來建立執行緒，這樣才能符合跨平台的開發需求。

使用跨平台的System.Threading.Tasks.Task類別來建立的執行緒，並且使用這個執行緒來定時執行Callback函式，這就完成了跨平台Timer類別的功能設計。

###模組下載###

程式碼下載：[由此進入GitHub後，點選右下角的「Download ZIP」按鈕下載。](https://github.com/Clark159/CLK)

(開啟程式碼的時候，建議使用Visual Studio所提供的「大綱->摺疊至定義」功能來摺疊程式碼，以能得到較適合閱讀的排版樣式。)

###物件程式###

	using CLK.Diagnostics;
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	
	namespace CLK.Threading
	{
	    public sealed class PortableTimer : IDisposable
	    {
	        // Fields
	        private readonly ManualResetEvent _executeThreadEvent = new ManualResetEvent(false);
	
	        private readonly Action _callback = null;
	
	        private readonly int _interval = 0;
	
	
	        // Constructors
	        public PortableTimer(Action callback, int interval)
	        {
	            #region Contracts
	
	            if (callback == null) throw new ArgumentNullException();
	
	            #endregion
	
	            // Require
	            if (interval <= 0) throw new ArgumentException();
	
	            // Arguments
	            _callback = callback;
	            _interval = interval;
	
	            // Begin
	            Task.Factory.StartNew(this.Execute);
	        }
	
	        public void Dispose()
	        {
	            // End
	            _executeThreadEvent.Set();
	        }
	
	
	        // Methods
	        private void Execute()
	        {
	            while (true)
	            {
	                // Wait
	                if (_executeThreadEvent.WaitOne(_interval) == true)
	                {
	                    return;
	                }
	
	                // Execute
	                try
	                {
	                    // Callback
	                    _callback();
	                }
	                catch (Exception ex)
	                {
	                    // Fail
	                    DebugContext.Current.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "Callback", "Exception", ex.Message));
	                }
	            }
	        }
	    }
	}


##使用範例##

###CLK.Threading.Samples.No001 - 在Windows Store App中使用PortableTimer###

- 使用範例

		using System;
		using System.Threading;
		using Windows.UI.Xaml;
		using Windows.UI.Xaml.Controls;
		
		namespace CLK.Threading.Samples.No001
		{
		    public sealed partial class MainPage : Page
		    {
		        // Fields
		        private readonly object _syncRoot = new object();
		
		        private readonly SynchronizationContext _syncContext = null;
		
		        private PortableTimer _operateTimer = null;
		
		
		        // Constructors
		        public MainPage()
		        {
		            // Base
		            this.InitializeComponent();
		
		            // SyncContext
		            _syncContext = SynchronizationContext.Current;
		        }
		
		
		        // Handlers
		        private void MainPage_Loaded(object sender, RoutedEventArgs e)
		        {
		            lock (_syncRoot)
		            {
		                // Require
		                if (_operateTimer != null) return;
		
		                // Begin
		                _operateTimer = new PortableTimer(this.Timer_Ticked, 500);
		            }
		        }
		
		        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
		        {
		            lock (_syncRoot)
		            {
		                // Require
		                if (_operateTimer == null) return;
		
		                // End
		                _operateTimer.Dispose();
		                _operateTimer = null;
		            }
		        }
		
		        private void Timer_Ticked()
		        {
		            System.Threading.SendOrPostCallback methodDelegate = delegate(object state)
		            {
		                // Display            
		                this.TextBlock001.Text = DateTime.Now.ToString();
		            };
		            _syncContext.Post(methodDelegate, null);
		        }
		    }
		}

- 執行結果

	![使用範例01](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Threading/PortableTimer/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B01.png)


###CLK.Threading.Samples.No002 - 在Windows Phone App中使用PortableTimer###

- 使用範例

		using System;
		using System.Windows;
		using Microsoft.Phone.Controls;
		using System.Threading;
		
		namespace CLK.Threading.Samples.No002
		{
		    public partial class MainPage : PhoneApplicationPage
		    {
		        // Fields
		        private readonly object _syncRoot = new object();
		
		        private readonly SynchronizationContext _syncContext = null;
		
		        private PortableTimer _operateTimer = null;
		
		
		        // Constructors
		        public MainPage()
		        {
		            // Base
		            this.InitializeComponent();
		
		            // SyncContext
		            _syncContext = SynchronizationContext.Current;
		        }
		
		
		        // Handlers
		        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		        {
		            lock (_syncRoot)
		            {
		                // Require
		                if (_operateTimer != null) return;
		
		                // Begin
		                _operateTimer = new PortableTimer(this.Timer_Ticked, 500);
		            }
		        }
		
		        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
		        {
		            lock (_syncRoot)
		            {
		                // Require
		                if (_operateTimer == null) return;
		
		                // End
		                _operateTimer.Dispose();
		                _operateTimer = null;
		            }
		        }
		
		        private void Timer_Ticked()
		        {
		            System.Threading.SendOrPostCallback methodDelegate = delegate(object state)
		            {
		                // Display            
		                this.TextBlock001.Text = DateTime.Now.ToString();
		            };
		            _syncContext.Post(methodDelegate, null);
		        }
		    }
		}

- 執行結果

	![使用範例02](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Threading/PortableTimer/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B02.png)


###CLK.Threading.Samples.No003 - 在WPF中使用PortableTimer###

- 使用範例

		using System;
		using System.Threading;
		using System.Windows;
		
		namespace CLK.Threading.Samples.No003
		{
		    public partial class MainWindow : Window
		    {
		        // Fields
		        private readonly object _syncRoot = new object();
		
		        private readonly SynchronizationContext _syncContext = null;
		
		        private PortableTimer _operateTimer = null;
		
		
		        // Constructors
		        public MainWindow()
		        {
		            // Base
		            this.InitializeComponent();
		
		            // SyncContext
		            _syncContext = SynchronizationContext.Current;
		        }
		
		
		        // Handlers
		        private void Window_Loaded(object sender, RoutedEventArgs e)
		        {
		            lock (_syncRoot)
		            {
		                // Require
		                if (_operateTimer != null) return;
		
		                // Begin
		                _operateTimer = new PortableTimer(this.Timer_Ticked, 500);
		            }
		        }
		
		        private void Window_Unloaded(object sender, RoutedEventArgs e)
		        {
		            lock (_syncRoot)
		            {
		                // Require
		                if (_operateTimer == null) return;
		
		                // End
		                _operateTimer.Dispose();
		                _operateTimer = null;
		            }
		        }
		
		        private void Timer_Ticked()
		        {
		            System.Threading.SendOrPostCallback methodDelegate = delegate(object state)
		            {
		                // Display            
		                this.TextBlock001.Text = DateTime.Now.ToString();
		            };
		            _syncContext.Post(methodDelegate, null);
		        }
		    }
		}


- 執行結果

	![使用範例03](https://raw.github.com/Clark159/CLK/master/Documents/CLK.Threading/PortableTimer/Images/%E4%BD%BF%E7%94%A8%E7%AF%84%E4%BE%8B03.png)
