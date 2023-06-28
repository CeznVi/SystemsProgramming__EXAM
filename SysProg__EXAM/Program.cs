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

        private const int minLenghtPass = 3;

        /// <summary>
        /// Переменная для хранения пароля который успешно подобран
        /// </summary>
        private static string realPassword = "";

        /// <summary>
        /// Строка допустимых символов которые могут содержаться в пароле
        /// </summary>
        private const string allPossibleSymbol = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*_";


        private static void SravniPass(string password) 
        {
                if (password == "156q")
                    realPassword = password;
                else
                    return;
        }


        static void Main(string[] args)
        {

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //GeneratePass();
            //sw.Stop();

            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("Выполнение опараций АСИНХРОННО");
            //Console.WriteLine($"Реальный пароль: {realPassword}");
            //Console.WriteLine($"Затрачено времени {sw.Elapsed.TotalSeconds} сек.");
            //Console.ResetColor();


            //realPassword = "";
            //Console.WriteLine();

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
            catch (Exception ex)
            {
                //Console.WriteLine(pass.ToString());
                //Console.WriteLine(ex.Message);
            }
        }


        public static void GeneratePass()
        {
            List<Task> tasks = new List<Task>();
            

            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string pass1generation = allPossibleSymbol[i].ToString();

                tasks.Add(
                    Task.Run(() => { PassGenerateOneMore(pass1generation); })
                    );

                ////tasks.Add(Task.Factory.StartNew(() => PassGenerateOneMore(pass1generation)));
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static void GeneratePassSync()
        {
            List<Task> tasks = new List<Task>();


            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string pass1generation = allPossibleSymbol[i].ToString();

                Task t = new Task(() => PassGenerateOneMore(pass1generation));
                tasks.Add(t);
                t.RunSynchronously();

            }
            Task.WaitAll(tasks.ToArray());
        }



        /// <summary>
        /// Метод который порождает генарацию потоков и осуществляет подбор пароля
        /// </summary>
        /// <param name="pass"></param>
        //public async static void PassGenerateOneMore(object pass)
        //{
        //    if (realPassword.Length > 0 || pass.ToString().Length > maxLenghtPass)
        //        return;

        //    //Console.WriteLine(pass);

        //    //TryCheckPassword(pass);

        //    //SravniPass(pass.ToString());

        //    ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
        //    if (realPassword.Length > 0)
        //        return;

        //    //// "ручной тормоз" останавливает подбор паролей в случае привышения МАКС. длины пароля
        //    if (pass.ToString().Length >= maxLenghtPass)
        //        return;

        //    for (int i = 0; i < allPossibleSymbol.Length; i++)
        //    {
        //        string passNextDepth = pass.ToString() + allPossibleSymbol[i].ToString();


        //        if (passNextDepth.Length >= minLenghtPass)
        //            SravniPass(passNextDepth);

        //        //if (passNextDepth.Length >= minLenghtPass)
        //        //    TryCheckPassword(passNextDepth);


        //        ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
        //        if (realPassword.Length < 0 || passNextDepth.Length <= maxLenghtPass)
        //            await Task.Run(() => PassGenerateOneMore(passNextDepth));
        //        else
        //            break;

        //    }

        //}



        public static void PassGenerateOneMore(object pass)
        {

            if (realPassword.Length > 0 || pass.ToString().Length > maxLenghtPass)
                return;

            //Console.WriteLine(pass);

            TryCheckPassword(pass);

            //SravniPass(pass.ToString());

            ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
            //if (realPassword.Length > 0)
            //    return;

            //// "ручной тормоз" останавливает подбор паролей в случае привышения МАКС. длины пароля
            if (pass.ToString().Length >= maxLenghtPass)
                return;

            List<Task> _activeTaskList = new List<Task>();
            

            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string passNextDepth = pass.ToString() + allPossibleSymbol[i].ToString();

                //if (passNextDepth.Length >= minLenghtPass)
                //    SravniPass(passNextDepth);

                if (passNextDepth.Length >= minLenghtPass)
                    TryCheckPassword(passNextDepth);

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

        public static void PassGenerateOneMoreSync(object pass)
        {

            if (realPassword.Length > 0 || pass.ToString().Length > maxLenghtPass)
                return;

            //Console.WriteLine(pass);

            TryCheckPassword(pass);

            //SravniPass(pass.ToString());

            ///"Ручной тормоз" если пароль подобран то остановить порождение потоков
            //if (realPassword.Length > 0)
            //    return;

            //// "ручной тормоз" останавливает подбор паролей в случае привышения МАКС. длины пароля
            if (pass.ToString().Length >= maxLenghtPass)
                return;

            List<Task> _activeTaskList = new List<Task>();


            for (int i = 0; i < allPossibleSymbol.Length; i++)
            {
                string passNextDepth = pass.ToString() + allPossibleSymbol[i].ToString();

                //if (passNextDepth.Length >= minLenghtPass)
                //    SravniPass(passNextDepth);

                if (passNextDepth.Length >= minLenghtPass)
                    TryCheckPassword(passNextDepth);

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