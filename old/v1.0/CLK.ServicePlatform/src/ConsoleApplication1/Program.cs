using CLK.ServiceApplication.Authentication;
using CLK.ServiceApplication.Authentication.Password;
using CLK.ServiceApplication.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            try
            {
                AuthenticationContext context = new AuthenticationContext(new MockAuthenticationTicketRepository());
                context.Attach(new MockPasswordAuthenticationProvider());
                
                Console.WriteLine((await context.SignInAsync("Clark", "123")).Succeeded);

                Console.WriteLine((await context.SignInAsync("Warning", "123")).Succeeded);

                //Console.WriteLine((await context.SignInAsync("Exception", "123")).Succeeded);


                Console.WriteLine((await context.SignInAsync()).Succeeded);
                context.SignOut();
                Console.WriteLine((await context.SignInAsync()).Succeeded);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            

            Console.WriteLine("End...");
            Console.ReadLine();
        }
    }
    
    public class MockPasswordAuthenticationProvider : PasswordAuthenticationProvider
    {
        protected override Task<AuthenticateResult> AuthenticateAsync(string id, string password)
        {
            //Console.WriteLine(string.Format("id:={0}, pw={1}", id, password));
            
            if (id == "Exception")
            {
                throw new Exception("GGG");
            }

            if (id == "Warning")
            {
                return Task.FromResult<AuthenticateResult>(AuthenticateResult.Fail(new Exception(id)));
            }

            AuthenticationTicket ticket = new AuthenticationTicket(new MockUser(id));
            return Task.FromResult<AuthenticateResult>(AuthenticateResult.Success(ticket));
        }
    }


    public class MockAuthenticationTicketRepository : AuthenticationTicketRepository
    {
        private AuthenticationTicket _ticket = null;

        public AuthenticationTicket GetTicket()
        {
            return _ticket;
        }

        public void RemoveTicket()
        {
            _ticket = null;
        }

        public void SetTicket(AuthenticationTicket ticket)
        {
            _ticket = ticket;
        }
    }

    public class MockUser : User
    {
        public MockUser(string name)
        {
            this.Name = name;
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name { get; private set; }
    }
}
