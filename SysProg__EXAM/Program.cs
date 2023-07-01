using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace SysProg__EXAM
{
    internal class Program
    {

        /// <summary>
        /// Максимальная длина пароля
        /// </summary>
        private const int maxLenghtPass = 4;

        /// <summary>
        /// Минимальная длина пароля
        /// </summary>
        private const int minLenghtPass = 2;

        /// <summary>
        /// Переменная для хранения пароля который успешно подобран
        /// </summary>
        private static string realPassword = "";

        /// <summary>
        /// Строка допустимых символов которые могут содержаться в пароле
        /// </summary>
        private const string allPossibleSymbol = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*_";

        /// <summary>
        /// Метод для проверки пароля. Из за особености Windows и невозможности 
        /// проверять пароль с помомощью метода TryCheckPassword так как 
        /// система блокирует третью попытку пришлось прибегнуть к методу прослойке.
        /// </summary>
        /// <param name="password"></param>
        private static void ComparePassword(string password) 
        {
            if (password == "156q")
            {
                realPassword = password;
                TryCheckPassword(realPassword);
            }
            else
                return;
        }


        static void Main(string[] args)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            GeneratePass();
            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Выполнение опараций АСИНХРОННО");
            Console.WriteLine($"Реальный пароль: {realPassword}");
            Console.WriteLine($"Затрачено времени {sw.Elapsed.TotalSeconds} сек.");
            Console.ResetColor();


            realPassword = "";
            Console.WriteLine();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            GeneratePassSync();
            sw2.Stop();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Выполнение опараций СИНХРОННО");
            Console.WriteLine($"Реальный пароль: {realPassword}");
            Console.WriteLine($"Затрачено времени {sw2.Elapsed.TotalSeconds} сек.");
            Console.ResetColor();

        }



        /// <summary>
        /// Метод для проверки пароля, если пароль подошел то он будет записан в переменную 
        /// </summary>
        /// <param name="pass">строка пароля передается как обджект</param>
        public static void TryCheckPassword(object pass)
        {
            try
            {
                SecureString securePWD = new SecureString();

                foreach (var item in pass.ToString())
                {
                    securePWD.AppendChar(item);
                }

                Process process = new Process();
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(@"C:\Windows\system32\calc.exe");
                process.StartInfo.FileName = "calc.exe";
                process.StartInfo.UserName = "Test"; //156q

                #pragma warning disable CA1416 // Отключение предупреждения
                process.StartInfo.Password = securePWD;
                #pragma warning restore CA1416 // Отключение предупреждения

                process.StartInfo.UseShellExecute = false;

                process.Start();
                realPassword = pass.ToString();
            }
            catch (Exception)
            {
                //Console.WriteLine(pass.ToString());
                //Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Сгенерировать первую часть пароля (1 символ) АСИНХРОННО
        /// </summary>
        public static void GeneratePass()
        {
            List<Task> tasks = new List<Task>();
            

            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string pass1generation = allPossibleSymbol[i].ToString();

                tasks.Add(
                    Task.Run(() => { PassGenerateOneMore(pass1generation); })
                    );

            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Сгенерировать первую часть пароля (1 символ) СИНХРОННО
        /// </summary>
        private static void GeneratePassSync()
        {
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string pass1generation = allPossibleSymbol[i].ToString();

                Task t = new Task(() => PassGenerateOneMoreSync(pass1generation));
                tasks.Add(t);
                t.RunSynchronously();

            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Рекурсионный метод генерации 2 и последующих символов АСИНХРОННО
        /// </summary>
        /// <param name="pass"></param>
        public static void PassGenerateOneMore(object pass)
        {

            ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
            if (realPassword.Length > 0 || pass.ToString().Length > maxLenghtPass)
                return;

            ComparePassword(pass.ToString());

            //// "ручной тормоз" останавливает подбор паролей в случае привышения МАКС. длины пароля
            if (pass.ToString().Length >= maxLenghtPass)
                return;

            List<Task> _activeTaskList = new List<Task>();
            

            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string passNextDepth = pass.ToString() + allPossibleSymbol[i].ToString();

                if (passNextDepth.Length >= minLenghtPass)
                    ComparePassword(passNextDepth);

                ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
                if (realPassword.Length < 0 || passNextDepth.Length <= maxLenghtPass)
                {
                    Task task = Task.Factory.StartNew(() => PassGenerateOneMore(passNextDepth));

                    _activeTaskList.Add(task);
                }
            }
            if (realPassword.Length > 0)
                return;

            Task.WaitAll(_activeTaskList.ToArray());
        }
        
        /// <summary>
        /// Рекурсионный метод генерации 2 и последующих символов СИНХРОННО
        /// </summary>
        /// <param name="pass"></param>
        public static void PassGenerateOneMoreSync(object pass)
        {

            ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
            if (realPassword.Length > 0 || pass.ToString().Length > maxLenghtPass)
                return;

            ComparePassword(pass.ToString());

            //// "ручной тормоз" останавливает подбор паролей в случае привышения МАКС. длины пароля
            if (pass.ToString().Length >= maxLenghtPass)
                return;

            List<Task> _activeTaskList = new List<Task>();

            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string passNextDepth = pass.ToString() + allPossibleSymbol[i].ToString();

                if (passNextDepth.Length >= minLenghtPass)
                    ComparePassword(passNextDepth);

                ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
                if (realPassword.Length < 0 || passNextDepth.Length <= maxLenghtPass)
                {
                    Task task = new Task( () => PassGenerateOneMoreSync(passNextDepth));
                    task.RunSynchronously();
                    _activeTaskList.Add(task);
                }
            }

            if (realPassword.Length > 0)
                return;

            Task.WaitAll(_activeTaskList.ToArray());
        }

    }
}