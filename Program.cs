namespace C__Test
{
    internal class Program
    {
        private const string XmlFileName = "C:\\Users\\hayru\\OneDrive\\Рабочий стол\\Test C#\\C# Test\\tags.xml"; // Имя файла для сохранения/загрузки XML
        private static TagStorage storage = new TagStorage(); // Экземпляр хранилища тегов
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Устанавливаем русскую культуру для текущего потока
            System.Globalization.CultureInfo russianCulture = new System.Globalization.CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentCulture = russianCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = russianCulture;

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Выгрузка дерева тегов из файла XML");
                Console.WriteLine("2. Загрузка дерева тегов в файл XML");
                Console.WriteLine("3. Вывод построчного списка тэгов на экран");
                Console.WriteLine("4. Удаление тэга по полному имени");
                Console.WriteLine("5. Добавление нового тэга");
                Console.WriteLine("6. Переименование тэга");
                Console.WriteLine("0. Выход");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoadTags(); // Загрузка тегов из XML
                        break;
                    case "2":
                        SaveTags(); // Сохранение тегов в XML
                        break;
                    case "3":
                        DisplayTags(); // Отображение всех тегов
                        break;
                    case "4":
                        RemoveTag(); // Удаление тэга по полному пути
                        break;
                    case "5":
                        AddTag(); // Добавление нового тэга
                        break;
                    case "6":
                        RenameTag(); // Переименование тэга
                        break;
                    case "0":
                        exit = true; // Выход из программы
                        break;
                    default:
                        Console.WriteLine("Введено некорректное значение!");
                        break;
                }
            }
        }
        static void LoadTags()
        {
            try
            {
                storage.LoadFromFile(XmlFileName);
                Console.WriteLine("Выгрузка дерева тегов прошла успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выгрузки тегов: {ex.Message}");
            }
        }
        static void SaveTags()
        {
            try
            {
                storage.SaveToFile(XmlFileName);
                Console.WriteLine("Загрузка дерева тегов прошла успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки тегов: {ex.Message}");
            }
        }
        static void DisplayTags()
        {
            foreach (var tag in storage.GetAllTags())
            {
                Console.WriteLine($"Путь тега: {tag.FullPath}, Уровень вложенности тега: {tag.Level}, Тип данных тега: {tag.valueType.Name}, Значение тега: {tag.GetTagValue()}");
            }
        }

        static void RemoveTag()
        {
            Console.Write("Введите полный путь тега, для удаления: ");
            string fullPath = Console.ReadLine();

            var tag = storage.FindTagByFullPath(fullPath);

            if (tag != null && tag != storage.Root)
            {
                tag.ParentTag.RemoveChildTag(tag.TagName);
                tag.UpdateFullpath();
                tag.ParentTag.UpdateFullpath();
                Console.WriteLine("Удаления тега прошло успешно.");
            }
            else
            {
                Console.WriteLine("Тег не найден, удаление не может быть произведено!");
            }
        }

        static void AddTag()
        {
            Console.Write("Введите полный путь родительского тега, для добавления нового: ");
            string parentPath = Console.ReadLine();

            var parent = storage.FindTagByFullPath(parentPath);
            if (parent != null)
            {
                Console.Write("Введите имя нового тега: ");
                string name = Console.ReadLine();

                Console.Write("Введите тип данных значения нового тега (int, double, bool, none): ");
                string type = Console.ReadLine();

                object value = null;

                if (type == "int")
                {
                    Console.Write("Введите значения типа int: ");
                    value = int.Parse(Console.ReadLine());
                }
                else if (type == "double")
                {
                    Console.Write("Введите значение типа double: ");
                    value = double.Parse(Console.ReadLine());
                }
                else if (type == "bool")
                {
                    Console.Write("Введите значение типа bool (true/false): ");
                    value = bool.Parse(Console.ReadLine());
                }

                var newTag = new TagItem(name, value);
                parent.AddChildTag(newTag);
                parent.UpdateFullpath();
                Console.WriteLine("Новый тег успешно добавлен.");
            }
            else
            {
                Console.WriteLine("Родительский тег не найден, невозможно выполнить добавление!");
            }
        }

        static void RenameTag()
        {
            Console.Write("Введите полный путь тега для переименования: ");
            string fullPath = Console.ReadLine();

            var tag = storage.FindTagByFullPath(fullPath);
            if (tag != null && tag != storage.Root)
            {
                //Console.Write("Старое имя тега: " + tag.TagName);
                Console.Write("Введите новое имя тега: ");
                string newName = Console.ReadLine();
                tag.TagName = newName;
                tag.UpdateFullpath();

                //Console.Write("Новое имя тега: " + tag.TagName);
                //Console.Write("Полный путь тега: " + tag.FullPath);
                Console.WriteLine("Переименование тега прошло успешно.");
            }
            else
            {
                Console.WriteLine("Тег не найден, не возможно выполнить переименование!");
            }
        }
    }
}
