using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

class Program
{



    // !! >>>>>> Эта часть нужна чтобы сделать окно во весь экран. Не знаю как работает, просто нашел в интернете и перепечатал  <<<<<<<<<<<
    /**/
    /**/
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto),/* SetLastError = true*/]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int MAXIMIZE = 3;
    /**/
    /**/
    // !! >>>>>> Эта часть нужна чтобы сделать окно во весь экран. Не знаю как работает, просто нашел в интернете и перепечатал  <<<<<<<<<<<


    static void Main(string[] args)
    {
        // !! >>>>>> Эта часть нужна чтобы сделать окно во весь экран. Не знаю как работает, просто нашел в интернете и перепечатал  <<<<<<<<<<<
        /**/
        /**/
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        ShowWindow(GetConsoleWindow(), MAXIMIZE);
        /**/
        /**/
        // !! >>>>>> Эта часть нужна чтобы сделать окно во весь экран. Не знаю как работает, просто нашел в интернете и перепечатал  <<<<<<<<<<<

        Console.Title = "Накорми Кота";

        Console.CursorVisible = false;
        const int MENU_MARGIN_LEFT = 20;        // Отступ для меню слева
        const int MENU_MARGIN_TOP = 2;          // Отсутп для меню сверху
        const int MENU_GAP_Y = 4;               // Отступ между кнопками меню (нельзя менять)
        const int BUTTON_SIZE = 30;             // Длина кнопки меню
        const int CAT_INFO_MARGIN_LEFT = 60;    // Отступ для поля Инфы о коте слева
        const int CAT_INFO_MARGIN_TOP = 10;     // Отступ для поля Инфы о коте сверху
        int catMarginTop = CAT_INFO_MARGIN_TOP + 10;
        string buttonLine = new string(' ', BUTTON_SIZE);
        string catInfoLine = new string(' ', 100);

        Random random = new Random();
        int fulnessBegin = random.Next(30);
        int fulnessEnd = random.Next(101);


        Food meal = new Food();
        int activeButton = 1;                                                  // Здесь будет храниться порядковый номер Активной кнопки
        int pressedButton = 0;                                                 // Здесь будет храниться порядковый номер Нажатой кнопки
        var foodCount = Enum.GetNames(typeof(Food)).Length;                    // Количесто элементов в списке Food
        string[] foodArray = { "Element number Zero" };                        // Создаю массив с одним элементом. К нему потом прибавлю массив Списка с едой, чтобы она нумеровалась с 1, а не с 0
        foodArray = foodArray.Concat(Enum.GetNames(typeof(Food))).ToArray();   // Создаю массив из Списка еды и добавляю его к предыдущему, в котором 1 элемент. 

        int catFulness = fulnessBegin;


        void Menu()
        {
            int countMenu = 1;

            foreach (string name in Food.GetNames(typeof(Food)))  //Рисуем меню: Перебираем по очереди каждый элемент списка Food. Если его значение совпадает с Активной кнопкой - выделить его другим цветом
            {
                if (name == foodArray[pressedButton])                                 // Выделяем нажатый пользователем пункт
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                }
                else if (name == foodArray[activeButton])                             // Выделяем активный пункт меню. 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                }

                else                                                                  // Остальные пункты рисуем одинаковыми
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Blue;
                }

                Console.SetCursorPosition(MENU_MARGIN_LEFT, MENU_MARGIN_TOP + MENU_GAP_Y * countMenu - 1); //1я строка пункта меню
                Console.Write(buttonLine);
                Console.SetCursorPosition(MENU_MARGIN_LEFT, MENU_MARGIN_TOP + MENU_GAP_Y * countMenu + 0); //2я строка пункта меню (центральная)
                Console.Write(buttonLine);

                Console.SetCursorPosition(MENU_MARGIN_LEFT, MENU_MARGIN_TOP + MENU_GAP_Y * countMenu + 1); //3я строка пункта меню
                Console.Write(buttonLine);

                Console.SetCursorPosition(MENU_MARGIN_LEFT + (BUTTON_SIZE - name.Length) / 2 + 1, MENU_MARGIN_TOP + MENU_GAP_Y * countMenu + 0); //надпись посередине кнопки
                Console.Write(name);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(MENU_MARGIN_LEFT, MENU_MARGIN_TOP + MENU_GAP_Y * countMenu + 2); //Разделитель между пунктами меню

                countMenu++;
            }
            pressedButton = 0;      // После нажатия на пункт меню и смещения выделения на другой пункт будет сбрасываться выделение (если не надо - убрать)

        }


        void ChooseFood()
        {
            Menu();
            CatInfo(catFulness);


            while (true)
            {
                var arrow = Console.ReadKey().Key;
                if (arrow == ConsoleKey.UpArrow) activeButton--;
                if (arrow == ConsoleKey.DownArrow) activeButton++;
                if (arrow == ConsoleKey.Escape) break;
                if (arrow == ConsoleKey.Enter) PressButton(activeButton);
                if (arrow == ConsoleKey.Spacebar) AutoFeeding();


                activeButton = activeButton < 1 ? foodCount : activeButton;             // Если выбрали пункт меньше первого - перейти в конец
                activeButton = activeButton > foodCount ? 1 : activeButton;             // Если выбрали пункт больше последнего - перейти в начало
                Menu();
                CatInfo(catFulness);

            }
        }

        void PressButton(int activeButton)
        {
            pressedButton = activeButton;
            meal = (Food)Enum.GetValues(typeof(Food)).GetValue(activeButton - 1);       // Задает элемент выбранного списка объекту списка
            catFulness = catFulness + (int)meal > 100 ? 100 : catFulness + (int)meal;   // Добавляет сытость коту соответственно "очкам сытости" выбранного элемента списка? Но не больше 100


        }

        void CatInfo(int fulness)
        {

            CatInfoLine(fulness);

            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP - 4); // Верхняя грань

            if (fulnessEnd < fulnessBegin && fulness == fulnessBegin) Console.WriteLine($" Кота необходимо было накорить до уровня сытости {fulnessEnd}. Но он уже переел на {fulness - fulnessEnd}. Коту пора сесть на диету ");

            else if (fulness < fulnessEnd) Console.WriteLine($" Кота необходимо накормить до уровня сытости {fulnessEnd}. Еще надо съесть на {fulnessEnd - fulness}                                      ");
            else if (fulness == fulnessEnd) Console.WriteLine($" Кота необходимо было накормить до уровня сытости {fulnessEnd}. Теперь он сыт                                                            ");
            else if (fulness > fulnessEnd) Console.WriteLine($" Кота необходимо было накорить до уровня сытости {fulnessEnd}. Но он уже переел на {fulness - fulnessEnd}                                 ");


            Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP - 2); // Верхняя грань
            Console.WriteLine(catInfoLine + "        ");

            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP - 1); // левая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP + 0); // левая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP + 1); // левая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP + 2); // левая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP + 3); // левая грань
            Console.WriteLine("  ");

            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 102, CAT_INFO_MARGIN_TOP - 1); // правая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 102, CAT_INFO_MARGIN_TOP + 0); // правая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 102, CAT_INFO_MARGIN_TOP + 1); // правая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 102, CAT_INFO_MARGIN_TOP + 2); // правая грань
            Console.WriteLine("  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 102, CAT_INFO_MARGIN_TOP + 3); // правая грань
            Console.WriteLine("  ");

            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT - 4, CAT_INFO_MARGIN_TOP + 4); // нижняя грань
            Console.WriteLine(catInfoLine + "        ");





        }

        void CatInfoLine(int fulness)   // Рисует полосу сытости 
        {
            string catInfoLine = new string(' ', fulness);

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT, CAT_INFO_MARGIN_TOP); // 1я строка
            Console.WriteLine(catInfoLine);
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT, CAT_INFO_MARGIN_TOP + 1); // 2я строка
            Console.WriteLine(catInfoLine);
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT, CAT_INFO_MARGIN_TOP + 2); // 3я строка
            Console.WriteLine(catInfoLine);

            if (catFulness < 52) Console.BackgroundColor = ConsoleColor.Black;
            if (catFulness > 51) Console.ForegroundColor = ConsoleColor.Black;              // Пишет % сытости. Меняет цвет когда на надпись заходит полоса сытости (баг когда заходит на Часть надписи фиксить не буду)
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 49, CAT_INFO_MARGIN_TOP + 1);
            Console.WriteLine(catFulness + "%");

            CatDraw(fulness);

        }


        void Draw(string line)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 20, catMarginTop);
            Console.WriteLine(line);
            catMarginTop++;
        }

        void CatDraw(int catMood)
        {
            catMarginTop = CAT_INFO_MARGIN_TOP + 10;
            if (catMood <= 20) catMood = 1;
            if (catMood > 20 && catMood <= 40) catMood = 2;
            if (catMood > 40 && catMood <= 60) catMood = 3;
            if (catMood > 60 && catMood <= 80) catMood = 4;
            if (catMood > 80) catMood = 5;


            switch (catMood)
            {

                case 1:

                    Draw("              *                                  *              ");
                    Draw("              *  *                            *  *              ");
                    Draw("              *     *     * * * * * * *    *     *              ");
                    Draw("              *        **               **       *              ");
                    Draw("              *                                  *              ");
                    Draw("              *                                  *              ");
                    Draw("             *                                    *             ");
                    Draw("            *       **                    **       *            ");
                    Draw("           *        *  *                *  *        *           ");
                    Draw("          *         *    *            *    *         *          ");
                    Draw("          *          *****   *****    *****          *          ");
                    Draw("          *                 *******                  *          ");
                    Draw("          *    --------------*****----------------   *          ");
                    Draw("           *    --------------***----------------   *           ");
                    Draw("            *                 / \\                  *            ");
                    Draw("             *          _____/   \\_____           *             ");
                    Draw("               *                                 *               ");
                    Draw("                 *                            *                 ");
                    Draw("                   ***                    ***                   ");
                    Draw("                       ******************                       ");
                    break;

                case 2:
                    Draw("              *                                  *              ");
                    Draw("              *  *                            *  *              ");
                    Draw("              *     *     * * * * * * *    *     *              ");
                    Draw("              *        **               **       *              ");
                    Draw("              *                                  *              ");
                    Draw("              *                                  *              ");
                    Draw("             *                                    *             ");
                    Draw("            *       ***                  ***       *            ");
                    Draw("           *        *   *              *   *        *           ");
                    Draw("          *         *    *            *    *         *          ");
                    Draw("          *          *****   *****    *****          *          ");
                    Draw("          *                 *******                  *          ");
                    Draw("          *    --------------*****----------------   *          ");
                    Draw("           *    --------------***----------------   *           ");
                    Draw("            *                 / \\                  *            ");
                    Draw("             *            ___/   \\___             *             ");
                    Draw("               *                                 *               ");
                    Draw("                 *                            *                 ");
                    Draw("                   ***                    ***                   ");
                    Draw("                       ******************                       ");
                    break;

                case 3:
                    Draw("              *                                  *              ");
                    Draw("              *  *                            *  *              ");
                    Draw("              *     *     * * * * * * *    *     *              ");
                    Draw("              *        **               **       *              ");
                    Draw("              *                                  *              ");
                    Draw("              *                                  *              ");
                    Draw("             *                                    *             ");
                    Draw("            *       ****                ****       *            ");
                    Draw("           *        *    *            *    *        *           ");
                    Draw("          *         *     *          *     *         *          ");
                    Draw("          *          ******  *****   ******          *          ");
                    Draw("          *                 *******                  *          ");
                    Draw("          *    --------------*****----------------   *          ");
                    Draw("           *    --------------***----------------   *           ");
                    Draw("            *                 / \\                  *            ");
                    Draw("             *          \\____/   \\___/            *             ");
                    Draw("               *                                 *               ");
                    Draw("                 *                            *                 ");
                    Draw("                   ***                    ***                   ");
                    Draw("                       ******************                       ");
                    break;

                case 4:
                    Draw("              *                                  *              ");
                    Draw("              *  *                            *  *              ");
                    Draw("              *     *     * * * * * * *    *     *              ");
                    Draw("              *        **               **       *              ");
                    Draw("              *                                  *              ");
                    Draw("              *                                  *              ");
                    Draw("             *                                    *             ");
                    Draw("            *    * ******              ****** *    *            ");
                    Draw("           *      *      *            *      *      *           ");
                    Draw("          *        **     *          *     **        *          ");
                    Draw("          *           *****  *****   *****            *          ");
                    Draw("         *                  *******                   *          ");
                    Draw("         *     --------------*****----------------    *          ");
                    Draw("          *     --------------***----------------    *           ");
                    Draw("            *                 / \\      /           *            ");
                    Draw("             *          \\____/   \\____/           *             ");
                    Draw("               *                                *               ");
                    Draw("                 *                            *                 ");
                    Draw("                   ***                    ***                   ");
                    Draw("                       ******************                       ");
                    break;

                case 5:
                    Draw("              *                                  *              ");
                    Draw("              *  *                            *  *              ");
                    Draw("              *     *     * * * * * * *    *     *              ");
                    Draw("              *        **               **       *              ");
                    Draw("              *                                  *              ");
                    Draw("              *                                  *              ");
                    Draw("             *                                    *             ");
                    Draw("            *    ***********          *********    *            ");
                    Draw("           *      ***********       **********       *           ");
                    Draw("         *                                             *          ");
                    Draw("        *                    *****                     *          ");
                    Draw("        *                   *******                    *           ");
                    Draw("        *      --------------*****----------------     *          ");
                    Draw("         *      --------------***----------------      *           ");
                    Draw("          *            /      / \\       \\             *            ");
                    Draw("            *          \\_____/   \\_____ /           *             ");
                    Draw("               *                                   *               ");
                    Draw("                 *                              **                 ");
                    Draw("                   ***                      ***                   ");
                    Draw("                       ********************                       ");
                    break;
            }
        }

        void AutoFeeding()
        {
            int[] autoFeed = new int[50];
            int autoCount = 0;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 88, CAT_INFO_MARGIN_TOP + 6 + autoCount);
            Console.WriteLine($"        Cat Auto-feeding mode:  ");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 88, CAT_INFO_MARGIN_TOP + 7 + autoCount);
            Console.WriteLine("-----------------------------------------");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 88, CAT_INFO_MARGIN_TOP + 8 + autoCount);
            Console.WriteLine($"|  №   |  What cat ate  | Fulness added |");
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 88, CAT_INFO_MARGIN_TOP + 9 + autoCount);
            Console.WriteLine("-----------------------------------------");                                 // Рисуем заголовок таблицы 

            while (catFulness <= /*fulnessEnd*/ 90)
            {
                autoCount++;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Black;

                int autoMeal = random.Next(foodCount);
                meal = (Food)Enum.GetValues(typeof(Food)).GetValue(autoMeal);                 // Задает случайно выбранный элемент списка объекту списка
                catFulness = catFulness + (int)meal > 100 ? 100 : catFulness + (int)meal;     // Добавляет сытость коту соответственно "очкам сытости" выбранного элемента списка? Но не больше 100
                int gap = foodArray[autoMeal + 1].Length;                                     // Длина слова, которое вписано в поле таблицы (на каждой строке создается заново)
                string spaceGap = new string(' ', (16 - gap) / 2);                            // Создает строку пробелов, равное ширине Поля в таблице минус длина слова, которое в него вписано
                string space1 = autoCount / 10 == 0 ? " " : "";                               // Добавляет строку в которой либо 0, либо 1 пробел, в зависимости от количества символов в числе первого столбца таблицы (на каждой строке своя)
                string space2 = gap % 2 == 1 ? " " : "";                                      // Добавляет строку в которой либо 0, либо 1 пробел, если в слове нечетное количество символов (на каждой строке своя)
                string space3 = (int)meal / 10 == 0 ? " " : "";                               // Добавляет строку в которой либо 0, либо 1 пробел, в зависимости от количества символов в числе последнего столбца таблицы (на каждой строке своя)


                Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 88, CAT_INFO_MARGIN_TOP + 9 + autoCount);
                Console.WriteLine($"|  {autoCount}  {space1}|{spaceGap}{foodArray[autoMeal + 1]}{spaceGap}{space2}|      +{(int)meal}   {space3}   |");
                Menu();
                CatInfo(catFulness);

                Thread.Sleep(500);
            }
            Console.SetCursorPosition(CAT_INFO_MARGIN_LEFT + 88, CAT_INFO_MARGIN_TOP + 10 + autoCount);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("-----------------------------------------");

        }


        ChooseFood();


        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Black;
    }
}
enum Food
{
    root = 1,
    grass = 2,
    vegetable = 3,
    bug = 3,
    lizzard = 4,
    egg = 5,
    fish = 6,
    mouse = 7,
    rat = 8,
    bird = 9,
    rabbit = 12
}
