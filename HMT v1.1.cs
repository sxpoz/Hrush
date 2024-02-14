using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace HMT_v1._0
{
    internal class Program
    {
        static void GenerateGuiltyResident(int toGenerate, ref List<string> initialGuiltyResidents, List<Resident> residents)
        {
            Random random = new Random();
            string residentToCheck = "";
            for (int i = 0; i < toGenerate; i++)
            {
                while (true)
                {
                    residentToCheck = residents[random.Next(0, residents.Count)].Name.ToString();
                    if (!initialGuiltyResidents.Contains(residentToCheck) && residentToCheck != "София")
                    {
                        initialGuiltyResidents.Add(residentToCheck);
                        break;
                    }
                }
            }
        }
        static List<Resident> GenerateResidents(int toGenerate, List<Resident> residents)
        {
            Random random = new Random();
            List<int> indexes = new List<int>();
            List<Resident> residentsOut = new List<Resident>();
            int index = 0;
            for (int i = 0; i < toGenerate; i++)
            {
                index = random.Next(0, residents.Count);
                if (!indexes.Contains(index)) indexes.Add(index);
                else i--;
                indexes.Sort();
            }
            foreach (int i in indexes)
            {
                //Console.Write(i + " "); 
                residentsOut.Add(residents[i]);
            }
            return residentsOut;
        }
        static Resident GenerateResidents(List<Resident> residents) //not barnie
        {
            Random random = new Random();
            int index = 0;
            while (true)
            {
                index = random.Next(0, residents.Count);
                if (residents[index].Name != "Барни") break;
            }
            return residents[index];
        }
        static bool KickResident(ref List<Resident> residents)
        {
            Console.Write("Введите имя жителя, которого нужно выселить (если передумали, оставте строчку пустой): ");
            string name = Console.ReadLine();
            if (name == "") return true;
            Resident resident = residents.Find(x => x.Name == name);
            if (residents.Remove(resident))
            {
                Console.Write($"Резидент {resident.Name} покинул дом. Нажмите любую клавишу для продолжения");
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.WriteLine($"Такой житель не проживает у Вас в доме! Попробуйте еще раз");
                return false;
            }
        }
        static void ShowResidents(List<Resident> residents)
        {
            int index = 1;
            Console.WriteLine("В Вашем доме проживают следующие жильцы:");
            foreach (Resident resident in residents)
            {
                Console.WriteLine($"\t{index}) {resident.Name} из {resident.Room} квартиры");
                index++;
            }
            Console.Write("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
        static bool CheckForSleeplessNight(ref Information info, int actionsRequired) //true - когда бессоная ночь
        {
            if (info.Time + actionsRequired > 6)
            {
                Console.WriteLine("Вы не можете это сделать даже с бессоной ночью!");
                return false;
            }
            if (info.Time + actionsRequired - 1 == 5)
            {
                while (true)
                {
                    Console.Write("Вы не будете спать ночь, Вы уверены, что хотите сделать это? Напишите \"Да\" или \"Нет\": ");
                    string str = Console.ReadLine().ToLower();
                    if (str == "да")
                    {
                        info.ChangeStress(15);
                        info.AddDebuff();
                        //info.AddTime(actionsRequired);
                        return true;
                    }
                    else if (str == "нет")
                    {
                        Console.CursorVisible = false;
                        Console.WriteLine("Вы решили спать ночью. Нажмите любую клавишу для продолжения.");
                        Console.ReadKey();
                        Console.CursorVisible = true;
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Формат ввода ответа был неверный, попробуйте еще раз!");
                        continue;
                    }
                }
            }
            else return true;
        }
        static int DiceRoller(int dice)
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.Write("Бросок кости: ");
            Random rand = new Random();
            int j = 0;
            for (int i = 0; i < 100000; i++)
            {
                j = rand.Next(1, dice + 1);
                Console.SetCursorPosition(14, 0);
                Console.Write(j);
            }
            Console.Clear();
            Console.Write("Бросок кости: ");
            Console.Write(j);
            Console.CursorVisible = true;
            Console.WriteLine();
            return j;
        }

        static void SaveGame(string saveName, Information info, List<Resident> residents, List<string> guessedGuiltyResidents, List<string> itemsToExchangeThisWeek, 
            bool paidDebt, bool notIdealInventory, bool firstTimeLootingRobert, bool knowChristofer, bool oneTimeMessage, bool knowReginald,
            int stressCounter, int stealCounter, int ignoredProblems, int SophiaVisits, int LouiseVisits, int countForPercent, int guiltyByThisTime, int wrongOccusations, int exchangedItems, int soldJunk, 
            double pillsThisWeek, double currentPercent)
        {
            Console.WriteLine();
            StreamWriter sw = new StreamWriter(saveName);
            sw.WriteLine($"information:{info.Money};{info.Debt};{info.Stress};{info.Proviant};{info.Time};{(int)info.Day};{info.Week};{info.Debuff};{(int)info.lastEvent}");
            sw.WriteLine($"inventory:{String.Concat<string>(info.inventory.Select(x => x + ",")).TrimEnd(',')}");
            string residentsString = "";
            foreach(Resident res in residents) residentsString += res.Name + '|' + String.Concat<string>(res.ItemsInRoom.Keys.Select(x => x + " ")).TrimEnd(' ') + ',';
            residentsString = residentsString.TrimEnd(',');
            sw.WriteLine($"residents:{residentsString}");
            sw.WriteLine($"guessedGuiltyResidents:{String.Concat<string>(guessedGuiltyResidents.Select(x => x + ",")).TrimEnd(',')}");
            sw.WriteLine($"itemsToExchangeThisWeek:{String.Concat<string>(itemsToExchangeThisWeek.Select(x => x + ",")).TrimEnd(',')}");
            sw.WriteLine($"paidDebt:{paidDebt}");
            sw.WriteLine($"notIdealInventory:{notIdealInventory}");
            sw.WriteLine($"firstTimeLootingRobert:{firstTimeLootingRobert}");
            sw.WriteLine($"knowChristofer:{knowChristofer}");
            sw.WriteLine($"oneTimeMessage:{oneTimeMessage}");
            sw.WriteLine($"knowReginald:{knowReginald}");
            sw.WriteLine($"stressCounter:{stressCounter}");
            sw.WriteLine($"stealCounter:{stealCounter}");
            sw.WriteLine($"ignoredProblems:{ignoredProblems}");
            sw.WriteLine($"SophiaVisits:{SophiaVisits}");
            sw.WriteLine($"LouiseVisits:{LouiseVisits}");
            sw.WriteLine($"countForPercent:{countForPercent}");
            sw.WriteLine($"guiltyByThisTime:{guiltyByThisTime}");
            sw.WriteLine($"wrongOccusations:{wrongOccusations}");
            sw.WriteLine($"exchangedItems:{exchangedItems}");
            sw.WriteLine($"soldJunk:{soldJunk}");
            sw.WriteLine($"pillsThisWeek:{pillsThisWeek}");
            sw.WriteLine($"currentPercent:{currentPercent}");
            sw.WriteLine($"KnowInfo:{String.Concat<string>(residents.Where(r => r.KnowInfo == true).Select(r => r.Name + ",")).TrimEnd(',')}");
            sw.WriteLine($"CameraState:{String.Concat<string>(residents.Where(r => r.CameraState == true).Select(r => r.Name + ",")).TrimEnd(',')}");
            Console.CursorVisible = false;
            Console.WriteLine("Игра была сохранена. Нажмите любую клавишу для продолжения");
            Console.ReadKey();
            Console.CursorVisible = true;
            sw.Close();
        }
        static void InfoFromCameras(Information info, List<Resident> residents)
        {
            //int posY = 0;
            //Console.SetCursorPosition(posX, posY++);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Установленые камеры показывают, кто из жильцов сейчас дома: ");
            //Console.SetCursorPosition(posX, posY);
            foreach (Resident res in residents.Where(r => r.CameraState == true))
            {
                if (res.TimeAtHome.Split(',').Select(x => Convert.ToInt32(x)).ToArray()[(int)info.Day].ToString().Contains($"{info.Time}"))
                {
                    Console.Write($"*{res.Name} ");
                    //Console.SetCursorPosition(posX, ++posY);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            //if (posY == 2) Console.WriteLine("\t*У Вас нет установленых камер!");
        }
        static void Main(string[] args)
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + $"\\Saves");
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + $"\\Saves");
            string savesPath = Directory.GetCurrentDirectory();
            string fileName = "";
            DirectoryInfo directoryInfo = new DirectoryInfo(savesPath);
            List<string> saves = directoryInfo.GetFiles().Select(f => f.Name).ToList();
            Console.WriteLine("Список сохранений:");
            for (int i = 0; i < saves.Count; i++)
            {
                Console.WriteLine($"\t{i + 1}) {saves[i]}");
            }
            Console.Write("Введите индекс сохранения, который нужно загрузить. В случае неверного ввода заргузится начальный сценарий: ");
            if (int.TryParse(Console.ReadLine(), out int val) && val > 0 && val <= saves.Count)
            {
                fileName = saves[val - 1];
            }
            else fileName = saves[0];
            StreamReader sr = new StreamReader(fileName);
            List<string> lines = new List<string>();
            string line = sr.ReadLine();
            while (line != null) 
            {
                List<string> temp = line.Split(':').ToList();
                temp.RemoveAt(0);
                lines.Add(temp[0]);
                temp.Clear();
                line = sr.ReadLine();
            }
            Information info = new Information(lines[0]);
            List<Resident> possibleResidents = new List<Resident>()
            {
                new Resident("Реджинальд", 5, "1234, 1234, 4, 1234, 4, 1234, 1", 0, new Dictionary<string, bool>
                {
                }),
                new Resident("София", 7, "4, 4, 4, 4, 4, 1234, 1234", 0, new Dictionary<string, bool>
                {
                }),
                new Resident("Луиза", 8, "1234, 1234, 12, 12, 12, 12, 12", 0, new Dictionary<string, bool>
                {
                }),
                new Resident("Роберт", 11, "1234, 1234, 1234, 1234, 4, 4, 4", 0, new Dictionary<string, bool>
                {
                }),
                new Resident("Барни", 17, "1234, 12, 1234, 12, 1234, 12, 1234", 0, new Dictionary<string, bool>
                {
                }),
                new Resident("Юна", 22, "4, 4, 1234, 123, 4, 1234, 123", 0, new Dictionary<string, bool>
                {
                }),
                new Resident("Франческо", 29, "12, 34, 4, 1234, 24, 13, 1", 0, new Dictionary<string, bool> //12 pon
                {
                }),
                new Resident("Кристофер", 30, "4, 4, 4, 4, 34, 1234, 1234", 0, new Dictionary<string, bool>
                {
                }),
            };
            List<Resident> residents = new List<Resident>();
            List<string> itemNames = new List<string>();
            //Console.WriteLine(lines[1]);
            if (lines[1].Length != 0) foreach (string s in lines[1].Split(new char[] { ',' , ' ' }, StringSplitOptions.RemoveEmptyEntries)) info.AddItem(s);
            foreach (string s in lines[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string name = s.Split('|')[0];
                string[] loot = s.Split('|')[1].Split(' ');
                //foreach(string asd in loot) Console.Write(asd);
                residents.Add(possibleResidents.Find(r => r.Name == name));
                foreach (string s1 in loot)
                {
                    bool boolValue = (s1 == "Оружие" || s1 == "Наркотики" || s1 == "Диск (запрещ)" || s1 == "Видеокассета (запрещ)" || s1 == "Пластинка (запрещ)");
                    if (!s1.Contains("брум")) residents.Last().ItemsInRoom.Add(s1, boolValue);
                    else residents.Last().ItemsInRoom.Add(s1, false);
                }
            }
            List<string> guessedGuiltyResidents = new List<string>();
            if (lines[3].Length != 0) foreach (string s in lines[3].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) guessedGuiltyResidents.Add(s);
            List<string> itemsToExchangeThisWeek = new List<string>();
            foreach (string s in lines[4].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) itemsToExchangeThisWeek.Add(s);
            //Console.WriteLine(itemsToExchangeThisWeek[0] + " " + itemsToExchangeThisWeek[1]);
            bool paidDebt = (lines[5].ToLower() == "true");
            bool notIdealInventory = (lines[6].ToLower() == "true");
            bool firstTimeLootingRobert = (lines[7].ToLower() == "true");
            bool knowChristofer = (lines[8].ToLower() == "true");
            bool oneTimeMessage = (lines[9].ToLower() == "true");
            bool knowReginald = (lines[10].ToLower() == "true");
            //Console.WriteLine(paidDebt); Console.WriteLine(notIdealInventory); Console.WriteLine(firstTimeLootingRobert); Console.WriteLine(knowChristofer); Console.WriteLine(oneTimeMessage); Console.WriteLine(knowReginald); //
            int stressCounter = Convert.ToInt32(lines[11]);
            int stealCounter = Convert.ToInt32(lines[12]);
            int ignoredProblems = Convert.ToInt32(lines[13]);
            int SophiaVisits = Convert.ToInt32(lines[14]);
            int LouiseVisits = Convert.ToInt32(lines[15]);
            int countForPercent = Convert.ToInt32(lines[16]);
            int guiltyByThisTime = Convert.ToInt32(lines[17]);
            int wrongOccusations = Convert.ToInt32(lines[18]);
            int exchangedItems = Convert.ToInt32(lines[19]);
            int soldJunk = Convert.ToInt32(lines[20]);
            double pillsThisWeek = Convert.ToDouble(lines[21]);
            double currentPercent = Convert.ToDouble(lines[22]);
            foreach (string s in lines[23].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) residents.Find(r => r.Name == s).ChangeStatus();
            foreach (string s in lines[24].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) residents.Find(r => r.Name == s).ChangeCameraState();
            /*Console.WriteLine(stressCounter); Console.WriteLine(stealCounter); Console.WriteLine(ignoredProblems); Console.WriteLine(SophiaVisits); 
            Console.WriteLine(LouiseVisits); Console.WriteLine(countForPercent); Console.WriteLine(guiltyByThisTime); Console.WriteLine(wrongOccusations); 
            Console.WriteLine(exchangedItems); Console.WriteLine(soldJunk); Console.WriteLine(pillsThisWeek); Console.WriteLine(currentPercent);*/
            bool eventOccured1 = false;
            int border = 0;
            string divider = new string('-', 85);
            sr.Close();

            Console.Write($"Был загружен {fileName}. Для перехода к игре нажмите любую клавишу");
            Console.ReadKey();

            Random rand = new Random();
            Store store = new Store();
            Dictionary<string, double> storeStock = store.RefreshStock(info);
            List<Occations> occations = new List<Occations> {
                new Occations("Драка", 20, 0, 10, 0.3, "Автоботы разберутся с дерущимеся, можете быть спокойны!"),
                new Occations("Электричество", 25, 0, 10, 0.2, "Автоботы починят электричество, можете быть спокойны!"),
                new Occations("Морозы", 25, 0, 5, 0.35, "Автоботы перейдут в режим нагрева помещений, у Вас в доме будет тепло!"),
                new Occations("Газ", 25, 5, 10, 0.5, "Автоботы найдут утечку и устранят ее, можете быть спокойны!"),
                new Occations("Затопление", 30, 5, 20, 0.3, "Автоботы найдут утечку и устранят ее, можете быть спокойны!"),
                new Occations("Задымление", 35, 5, 20, 0.4, "Автоботы найдут утечку и устранят ее, можете быть спокойны!"),
                new Occations("Пожар", 40, 10, 25, 0.6, "Автоботы переведены в режим тушения огня. После - они проведут легкий косметический ремонт и дом будет как новенький!"),
                new Occations("Забастовка", 40, 10, 35, 0, "Автоботы разберутся с демонстрантами, можете быть спокойны!"),
                new Occations("Обрушение", 45, 15, 40, 0.4, "Автоботы починят здание и спасут заваленных. Дом будет как новенький!"),
                new Occations("Подрыв", 60, 15, 45, 0.5, "Автоботы починят здание и спасут пострадавших. Дом будет как новенький!")
            };
            List<int> minimalNumberOfGuiltyResidents = new List<int> { 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4 };
            List<Chill> chillMethods = new List<Chill>
            {
                new Chill("Выпивка", 10, 0, new List<string> {"Пиво", "Виски"}, 0.1),
                new Chill("Просмотр телевизора", 20, 0, new List<string> {"Телевизор", "Видеокассета(легал)"}, 0),
                new Chill("Выпивка + просмотр телевизора", 35, 0, new List<string> { "Телевизор", "Видеокассета(легал)", "Пиво", "Виски"}, 0.15),
                new Chill("Поход в бар", 30, 40, new List<string> {}, 0.25),
                new Chill("Прием наркотиков", 100, 0, new List<string> {"Наркотики"}, 2.0),
                new Chill("Музыка", 15, 0, new List<string> {"Пластинка(легал)"}, 0),
                new Chill("Чтение журнала", 15, 0, new List<string> {"Журнал"}, 0)
            };
            List<bool> barnieSays = new List<bool> { true, false, true, true, true, false, false, true, false, true, false, false, false, false };
            Console.SetWindowSize(150, 60);
            while (info.Week < 15)
            {
                Console.Clear();
                if (info.Proviant == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("У вас закончился провиант. Вы будете получать стресс каждый новый день.\n");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                if (info.lastEvent != (Events)100)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Вам нужно решить следующию проблему: {info.lastEvent}. Обязательно нажмите 15 пункт в течении дня");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                info.ShowInfo();
                Console.WriteLine(divider);
                //Console.SetCursorPosition(0, 7 + info.inventory.Count / 7);
                Console.WriteLine("Какое действие Вы хотите сделать? Можно:\n" + divider + "\n\tI) Работа с информацией");
                Console.WriteLine("\t\t1) Добавить/снять деньги деньги\n\t\t2) Заплатить долг\n\t\t3) Изменить стресс\n\t\t4) Изменить кол-во провизии\n\t\t5) Добавить предмет в инвентарь\n\t\t6) Удалить предмет из инвентаря");
                Console.WriteLine(divider + "\n\tII) Жильцы");
                Console.WriteLine("\t\t7) Показать список жильцов\n\t\t8) Показать известные расписания жильцов\n\t\t9) Действие: поиск случайного жильца и беседа с ним\n\t\t10) Действие: поиск конкретного жильца и беседа с ним\n\t\t11) Действие: действие в комнате жильца\n\t\t12) Сохранить информацию о полученом расписании жильца\n\t\t13) Выселить жителя дома");
                Console.WriteLine(divider + "\n\tIII) Действия");
                Console.WriteLine("\t\t14) Действие: поход в магазин\n\t\t15) Действие: Решение экстренной ситуации/проблемы (нажимать обязательно)\n\t\t16) 2 Действия: Досуг\n\t\t17) 3 Действия: Написать еженедельный отчет\n\t\t18) 3 Действия: Подработка");
                Console.WriteLine(divider + "\n\tIV) Журнал");
                Console.WriteLine("\t\t19) Показать журнал\n\t\t20) Внести запись в журнал\n\t\t21) Удалить запись из журнала");
                Console.WriteLine(divider + "\n\tV) Остальное");
                Console.WriteLine("\t\t22) Пропустить день/остаток дня\n\t\t23) Изменить время\n\t\t24) Обновить информацию на экране\n\t\t25) Кинуть кубик\n\t\t26) Принять лекарства\n\t\t27) Сохранить игру");
                InfoFromCameras(info, residents);
                Console.Write("\nИспользовать бессоную ночь можно только на действия, которые требуют 2 и больше единицы времени\nВведите нужную цифру: ");
                
                switch (Console.ReadLine().ToLower())
                {
                    case "1":
                        {
                            while (true)
                            {
                                Console.Write("Введите кол-во полученых/потраченных брум: ");
                                if (int.TryParse(Console.ReadLine(), out int value))
                                {
                                    info.AddMoney(value);
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            break;
                        }
                    case "2":
                        {
                            while (true)
                            {
                                Console.Write("Введите сумму погашения долга, кратную 100-м брум. Минимальная сумма 100 брум: ");
                                if (int.TryParse(Console.ReadLine(), out int value) && value % 100 == 0)
                                {
                                    if (value == 0) break;
                                    info.PayDebt(value, true);
                                    if (paidDebt != true && value < 0) paidDebt = false;
                                    paidDebt = true;
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            break;
                        }
                    case "3":
                        {
                            while (true)
                            {
                                Console.Write("Введите полученный стресс. Для снятия стресса напишите отрицательное число: ");
                                if (int.TryParse(Console.ReadLine(), out int value))
                                {
                                    info.ChangeStress(value);
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            break;
                        }
                    case "4":
                        {
                            while (true)
                            {
                                Console.Write("Введите полученную провизию. Для уменьшения количества напишите отрицательное число: ");
                                if (int.TryParse(Console.ReadLine(), out int value))
                                {
                                    info.AddProviant(value);
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            break;
                        }
                    case "5":
                        {
                            info.AddItem();
                            break;
                        }
                    case "6":
                        {
                            info.RemoveItem();
                            break;
                        }
                    case "7":
                        {
                            ShowResidents(residents);
                            break;
                        }
                    case "8":
                        { 
                        
                            foreach (Resident resident in residents)
                            {
                                resident.ShowResident(info);
                                Console.WriteLine();
                            }

                            /*Console.Write("Введите полное имя жильца (если ошиблись, оставте строку пустой): ");
                            bool nameFound = false;
                            while (!nameFound)
                            {
                                string str = Console.ReadLine();
                                if (str == "") break;
                                foreach (Resident resident in residents)
                                    if (resident.Name == str)
                                    {
                                        resident.ShowResident();
                                        nameFound = true;
                                        break;
                                    }
                                if (!nameFound) Console.WriteLine("Жильец с таким именем не проживает у Вас в доме! Попробуйте еще раз");
                            }*/
                            Console.Write("Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                    case "9":
                        {
                            List<Resident> list = residents.Where(x => x.IsResidentHome(info.Time, info.Day) == true).ToList();
                            Random rand1 = new Random();
                            //foreach (Resident resident in list) { Console.WriteLine(resident.Name); }
                            string str = list[rand1.Next(0, list.Count)].Name;
                            Console.WriteLine($"{str} поговорил(а) с Вами");
                            if (str == "Реджинальд") knowReginald = true;
                            if (str == "Кристофер") knowChristofer = true;
                            if (str == "Юна" && info.Week == 13)
                            {
                                Console.BackgroundColor = ConsoleColor.Green;
                                Console.Write("Просушайте ведущего и, когда он скажет, нажмите любую клавишу для продолжения");
                                Console.ReadKey();
                                Console.BackgroundColor = ConsoleColor.Black;
                                if (info.Debuff <= 0) Console.WriteLine("Юна продолжила: \"Судя по твоему состоянию правда, тебе можно и снизить дозировку\"");
                                else Console.WriteLine("Юна продолжила: \"Что-то смотрю я на тебя, мой дорогой, и у меня такое чувство, что тебе нужно принимать больше лекарств!" +
                                    "Постарайся несколько недель, я буду держать за тебя кулачки\"");
                            }
                            if (str == "София" && (info.Week == 11 || info.Week == 13)) Console.WriteLine($"София сообщила Вам следующее число: {currentPercent}");
                            if (str == "Барни")
                            {
                                Random rand2 = new Random();
                                Resident barnieTarget = GenerateResidents(residents);
                                if (info.Week == 14) Console.WriteLine("Вы мне очень не понравились, как наш смотритель дома! Вы ничего не сделали для того, " +
                                    "чтобы нам стало жить лучше! Убирайтесь!” Когда Барни сказал это, он расплакался и убежал.");
                                if (barnieSays[info.Week - 1]) //truth
                                {
                                    List<string> itemsInRoom = barnieTarget.GiveItemsInRoom(residents);
                                    string item1 = itemsInRoom[rand2.Next(0, itemsInRoom.Count)];
                                    string item2 = item1;
                                    while (item1 == item2)
                                    {
                                        item2 = itemsInRoom[rand2.Next(0, itemsInRoom.Count)]; //решил убрать 2 предмет
                                    }
                                    Console.WriteLine($"Барни сказал Вам, что {barnieTarget.Name} держит у себя дома {item1}");
                                }
                                else //lie
                                {
                                    List<string> itemsInRoom = barnieTarget.GiveItemsInRoom(residents);
                                    List<string> itemsNotInRoom = new List<string>();
                                    foreach (string item in info.possibleItems.Keys)
                                    {
                                        if (itemsInRoom.Contains(item)) continue;
                                        else itemsNotInRoom.Add(item);
                                    }
                                    string item1 = itemsNotInRoom[rand2.Next(0, itemsInRoom.Count)];
                                    string item2 = item1;
                                    while (item1 == item2)
                                    {
                                        item2 = itemsInRoom[rand2.Next(0, itemsInRoom.Count)]; //решил убрать 2 предмет
                                    }
                                    Console.WriteLine($"Барни сказал Вам, что {barnieTarget.Name} держит у себя дома {item1}");
                                }
                            }
                            Console.CursorVisible = false;
                            info.AddTime();
                            Console.Write("Нажмите любую клавишу для продлжения");
                            Console.ReadKey();
                            Console.CursorVisible = true;
                            break;
                        }
                    case "10":
                        {
                            List<Resident> list = residents.Where(x => x.IsResidentHome(info.Time, info.Day) == true).ToList();
                            while (true)
                            {
                                Console.Write("Кого Вы хотите найти? Напишите полное имя: ");
                                string str = Console.ReadLine();
                                Resident resident = residents.Find(x => x.Name == str);
                                if (resident == null)
                                {
                                    Console.WriteLine("Такого имени нет в списке жильцов, попробуйте еще раз!");
                                    continue;
                                }
                                else resident = list.Find(x => x.Name == str);
                                if (resident != null)
                                {
                                    Console.WriteLine($"{resident.Name} оказался(лась) дома и побеседовал(а) с Вами");
                                    if (str == "Реджинальд") knowReginald = true;
                                    if (str == "Кристофер") knowChristofer = true;
                                    if (str == "Юна" && info.Week == 13)
                                    {
                                        Console.BackgroundColor = ConsoleColor.Green;
                                        Console.Write("Просушайте ведущего и, когда он скажет, нажмите любую клавишу для продолжения");
                                        Console.ReadKey();
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        if (info.Debuff <= 0) Console.WriteLine("Юна продолжила: \"Судя по твоему состоянию правда, тебе можно и снизить дозировку\"");
                                        else Console.WriteLine("Юна продолжила: \"Что-то смотрю я на тебя, мой дорогой, и у меня такое чувство, что тебе нужно принимать больше лекарств!" +
                                            "Постарайся несколько недель, я буду держать за тебя кулачки\"");
                                    }
                                    if (str == "София" && (info.Week == 11 || info.Week == 13)) Console.WriteLine($"София сообщила Вам следующее число: {currentPercent}");
                                    if (str == "Барни")
                                    {
                                        Random rand2 = new Random();
                                        Resident barnieTarget = GenerateResidents(residents);
                                        if (info.Week == 14) Console.WriteLine("Вы мне очень не понравились, как наш смотритель дома! Вы ничего не сделали для того, " +
                                            "чтобы нам стало жить лучше! Убирайтесь!” Когда Барни сказал это, он расплакался и убежал.");
                                        if (barnieSays[info.Week - 1]) //truth
                                        {
                                            List<string> itemsInRoom = barnieTarget.GiveItemsInRoom(residents);
                                            string item1 = itemsInRoom[rand2.Next(0, itemsInRoom.Count)];
                                            string item2 = item1;
                                            while (item1 == item2)
                                            {
                                                item2 = itemsInRoom[rand2.Next(0, itemsInRoom.Count)]; //решил убрать 2 предмет
                                            }
                                            Console.WriteLine($"Барни сказал Вам, что {barnieTarget.Name} держит у себя дома {item1}");
                                        }
                                        else //lie
                                        {
                                            List<string> itemsInRoom = barnieTarget.GiveItemsInRoom(residents);
                                            List<string> itemsNotInRoom = new List<string>();
                                            foreach (string item in info.possibleItems.Keys)
                                            {
                                                if (itemsInRoom.Contains(item)) continue;
                                                else itemsNotInRoom.Add(item);
                                            }
                                            string item1 = itemsNotInRoom[rand2.Next(0, itemsInRoom.Count)];
                                            string item2 = item1;
                                            while (item1 == item2)
                                            {
                                                item2 = itemsInRoom[rand2.Next(0, itemsInRoom.Count)]; //решил убрать 2 предмет
                                            }
                                            Console.WriteLine($"Барни сказал Вам, что {barnieTarget.Name} держит у себя дома {item1}");
                                        }
                                    }
                                    Console.CursorVisible = false;
                                    info.AddTime();
                                    Console.Write("Нажмите любую клавишу для продлжения");
                                    Console.ReadKey();
                                    Console.CursorVisible = true;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine($"Вы проходили достаточно долго, чтобы с уверенностью сказать, что {str} не дома.");
                                    Console.CursorVisible = false;
                                    info.AddTime();
                                    Console.Write("Нажмите любую клавишу для продлжения");
                                    Console.ReadKey();
                                    Console.CursorVisible = true;
                                    break;
                                }

                            }
                            break;
                        }
                    case "11":
                        {
                            List<Resident> list = residents.Where(x => x.IsResidentHome(info.Time, info.Day) == true).ToList();
                            while (true)
                            {
                                Console.Write("К кому Вы хотите пойти в комнату? Напишите имя (если ошиблись, то оставте строчку пустой): ");
                                
                                string str = Console.ReadLine();
                                if (str == "") break;
                                Resident resident = residents.Find(x => x.Name == str);
                                if (resident == null)
                                {
                                    Console.WriteLine("Такого имени нет в списке жильцов, попробуйте еще раз!");
                                    continue;
                                }
                                if (resident.Name == "Роберт" && !firstTimeLootingRobert)
                                {
                                    resident.SellOrExchange(ref info, ref exchangedItems, itemsToExchangeThisWeek);
                                    info.AddTime();
                                    break;
                                }
                                resident = list.Find(x => x.Name == str);
                                if (resident != null)
                                {
                                    Console.WriteLine($"{resident.Name} оказался(лась) дома");
                                    if (str == "София") SophiaVisits++;
                                    if (str == "Луиза") LouiseVisits++;
                                    if (str == "Кристофер" && knowChristofer)
                                    {
                                        if (soldJunk >= 10 && oneTimeMessage)
                                        {
                                            oneTimeMessage = false;
                                            Console.BackgroundColor = ConsoleColor.Green;
                                            Console.WriteLine("Просулшайте ведущего, Кристофер хочет Вам что-то сказать. Когда будете готовы, нажмите любую клавишу для продолжения");
                                            Console.BackgroundColor = ConsoleColor.Black;
                                            Console.ReadKey();
                                        }
                                        resident.SellJunk(ref info, ref soldJunk);
                                        info.AddTime();
                                        break;
                                    }
                                    Console.CursorVisible = false;
                                    info.AddTime();
                                    Console.Write("Нажмите любую клавишу для продлжения");
                                    Console.ReadKey();
                                    Console.CursorVisible = true;
                                    break;
                                }
                                else
                                {
                                    bool restartNeeded = false;
                                    resident = residents.Find(x => x.Name == str);
                                    Console.WriteLine($"{str} не дома, Вы одни в квартире. Что Вы хотите сделать?\n\t1) Обыскать квартиру с возможностью украсть предмет\n\t" +
                                                       "2) Подложить предмет\n\t3) Установить камеру\n\t4) Уйти");
                                    while (true)
                                    {
                                        Console.Write("Введите число: ");
                                        if (int.TryParse(Console.ReadLine(), out int value))
                                        {
                                            switch (value)
                                            {
                                                case 1:
                                                    {
                                                        if (resident.Name == "Роберт" && firstTimeLootingRobert)
                                                        {
                                                            firstTimeLootingRobert = false;
                                                            Console.BackgroundColor = ConsoleColor.Red;
                                                            Console.WriteLine("Во время обыска домой вернулась жена Роберта. Прослушайте ведущего игры.");
                                                            Console.BackgroundColor = ConsoleColor.Black;
                                                            resident.Loot();
                                                            break;
                                                        }
                                                        resident.Loot();
                                                        string stolenItem = resident.Steal(ref stealCounter);
                                                        if (stolenItem != "") info.AddItem(stolenItem);
                                                        restartNeeded = false;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        resident.AddLootToRoom(info, ref stealCounter);
                                                        restartNeeded = false;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        resident.PlaceCamera(info, ref restartNeeded);
                                                        break;
                                                    }
                                                case 4:
                                                    {
                                                        restartNeeded = false;
                                                        break;
                                                    }
                                            }
                                            if (restartNeeded)
                                            {
                                                Console.WriteLine($"Вы все еще в комнате резидента {str}");
                                                continue;
                                            }
                                            break;
                                        }
                                        else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                                    }
                                    Console.CursorVisible = false;
                                    info.AddTime();
                                    Console.Write("Вы сделали то, что хотели. Нажмите любую клавишу для продлжения");
                                    Console.ReadKey();
                                    Console.CursorVisible = true;
                                    break;
                                }
                            }
                            break;
                        }
                    case "12":
                        {
                            Console.Write("Чью информацию сохранить? Введите имя. Если Вы получили информацию сразу по всем жильцам напишите \"все\": ");
                            bool nameFound = false;
                            while (!nameFound)
                            {
                                string str = Console.ReadLine();
                                if (str.ToLower() == "все")
                                {
                                    foreach (Resident resident in residents)
                                    {
                                        if (resident.KnowInfo == false) resident.ChangeStatus();
                                        nameFound = true;
                                    }
                                }
                                foreach (Resident resident in residents)
                                    if (resident.Name == str)
                                    {
                                        resident.ChangeStatus();
                                        nameFound = true;
                                        break;
                                    }
                                if (!nameFound) Console.WriteLine("Имя неверное, попробуйте еще раз!");
                            }
                            break;
                        }
                    case "13":
                        {
                            while (!KickResident(ref residents)) { }
                            break;
                        }
                    case "14":
                        {
                            if (info.Time > 3)
                            {
                                Console.WriteLine("Слишком поздно идти в магазин, он закрыт!");
                                Console.CursorVisible = false;
                                Console.Write("Нажмите любую клавишу для продлжения");
                                Console.ReadKey();
                                Console.CursorVisible = true;
                                break;
                            }
                            Console.WriteLine("Вы пришли в магазин и видите следующие товары: ");
                            while (true)
                            {
                                int i = 1;
                                Console.WriteLine(new string('-', 20));
                                foreach (var item in storeStock)
                                {
                                    Console.WriteLine($"{i}) {item.Key} ({item.Value} брум)");
                                    i++;
                                }
                                Console.WriteLine(new string('-', 20));
                                Console.Write($"Введите номер товара, который хотите купить. Для выхода из магазина введите \"0\": ");
                                string str = Console.ReadLine();
                                Console.WriteLine();
                                if (int.TryParse(str, out int value) && value != 0 && value <= storeStock.Count)
                                {
                                    if (storeStock.ElementAt(value - 1).Value > info.Money)
                                    {
                                        Console.WriteLine("У вас недостаточно денег!");
                                        continue;
                                    }
                                    else
                                    {
                                        info.AddMoney((int)-storeStock.ElementAt(value - 1).Value);
                                        if (storeStock.ElementAt(value - 1).Key.StartsWith("Провизия")) info.AddProviant();
                                        else if (storeStock.ElementAt(value - 1).Key.StartsWith("Суп")) info.AddProviant(5);
                                        else info.AddItem(storeStock.ElementAt(value - 1).Key);
                                        storeStock.Remove(storeStock.ElementAt(value - 1).Key);
                                        Console.WriteLine($"Оставшиеся деньги: {info.Money}");
                                    }
                                }
                                else if (value != 0)
                                {
                                    Console.WriteLine("Такого товара нет!");
                                    continue;
                                }
                                else if (str != "0")
                                {
                                    Console.WriteLine("Такого товара нет!");
                                    continue;
                                }
                                else break; 
                            }
                            Console.WriteLine($"Вы вернулись домой");
                            Console.CursorVisible = false;
                            info.AddTime();
                            Console.Write("Нажмите любую клавишу для продлжения");
                            Console.ReadKey();
                            Console.CursorVisible = true;
                            break;
                        }
                    case "15":
                        {
                            Occations occasion = new Occations();
                            foreach(Occations occ in occations)
                            {
                                if (info.lastEvent.ToString() == occ.Name) occasion = occ;
                            }
                            int count = 0;
                            Console.WriteLine($"Проблема: {info.lastEvent}. Напишите, как изменились Ваши ресурсы, а если проблема была проигнорирована, напишите 0 во ВСЕ поля:");
                            Console.WriteLine($"Для решения проблемы автоботами нужно:\n\t*Деньги: {occasion.BotsMoney}\n\t*Вы получите {occasion.BotsStress} стресса\n\t*Без вредя для здоровья");
                            Console.WriteLine(occasion.BotsMessage);
                            Console.WriteLine();
                            Console.WriteLine($"Для решения проблемы самому нужно:\n\t*Деньги: бесплатно\n\t*Вы получите {occasion.Stress} стресса\n\t*С вредом для здоровья");
                            if (knowReginald && info.lastEvent == Events.Электричество)
                            {
                                Console.WriteLine("Если у Вас хорошие отношения с Реджинальдом, он готов Вам помочь за 1 брум");
                            }
                            while (true)
                            {
                                Console.Write("Деньги (напишите отрицательное число): ");
                                if (int.TryParse(Console.ReadLine(), out int value))
                                {
                                    if (!info.AddMoneyBool(value))
                                    {
                                        continue;
                                    }
                                    if (value == 0) count++;
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            while (true)
                            {
                                Console.Write("\nСтресс: ");
                                if (int.TryParse(Console.ReadLine(), out int value))
                                {
                                    info.ChangeStress(value);
                                    if (value == 0) count++;
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            while (true)
                            {
                                Console.Write("\nБыл ли нанесен вред здоровью? 1 - Да, 2 - Нет: ");
                                if (int.TryParse(Console.ReadLine(), out int value))
                                {
                                    if (value == 1) info.AddDebuff(occasion.Debuff);
                                    if (value == 0) count++;
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            if (count == 3)
                            {
                                Console.WriteLine("Проблема была проигнорирована. Нажмите любую клавишу для продолжения.");
                                ignoredProblems++;
                                count = 0;
                                info.lastEvent = (Events)100;
                                Console.ReadKey();
                                break;
                            }
                            info.AddTime();
                            count = 0;
                            info.lastEvent = (Events)100;
                            break;
                        }
                    case "16":
                        {
                            if (CheckForSleeplessNight(ref info, 2))
                            {
                                Console.WriteLine("Возможные способы досуга:"); 
                                for (int i = 0; i < chillMethods.Count; i++) 
                                    Console.WriteLine($"\t{i + 1}) {chillMethods[i].ChillType} \n\t\tДля этого досуга нужно иметь: {chillMethods[i].ShowRequiredItems()}\n\t\t*Это действие снимет {chillMethods[i].StressRelief} ед. стресса*");
                                while (true)
                                {
                                    Console.Write("Введите номер выбранного досуга, если Вы передумали, введите \"0\": ");
                                    if (int.TryParse(Console.ReadLine(), out int index))
                                    {
                                        if (index == 0) break;
                                        if (index > chillMethods.Count)
                                        {
                                            Console.WriteLine("Такого способа нет, попробуйте еще раз!");
                                            continue;
                                        }
                                        Chill selectedChill = chillMethods.ElementAt(index - 1);
                                        selectedChill.DoChill(ref info);
                                        break;
                                    }
                                    else Console.WriteLine("Формат ввода был неверный, попробуйте еще раз!");
                                }
                                //Console.ReadKey();
                            }
                            break;
                        }
                    case "17":
                        {
                            if (info.Day != DayOfTheWeek.ВС)
                            {
                                Console.Write("Вы можете писать еженедельный отчет только в ВС. Нажмите любую клавишу для продолжения");
                                Console.ReadKey();
                                break;
                            }
                            if (CheckForSleeplessNight(ref info, 3))
                            {
                                int guiltyNumber = 0;
                                int reportsNumber = 0;
                                int matches = 0;
                                List<string> guiltyResidents = new List<string>();
                                List<string> residentNames = residents.Select(x => x.Name).ToList();
                                
                                bool wrongNames = true;
                                foreach (Resident resident in residents) resident.FillGuiltyResidents(ref guiltyResidents);
                                if (guiltyResidents.Count < minimalNumberOfGuiltyResidents[info.Week - 1]) GenerateGuiltyResident(minimalNumberOfGuiltyResidents[info.Week - 1] - guiltyResidents.Count, ref guiltyResidents, residents);
                                guiltyNumber = guiltyResidents.Count;
                                Console.WriteLine($"Вы решили заполнить еженедельный отчет. Число жителей, нарушивших закон на этой неделе: {guiltyNumber}");
                                Console.Write($"Установленные камеры позволили Вам распознать следующих нарушителей: ");
                                foreach (string name in guiltyResidents)
                                {
                                    if (residents.Find(x => x.Name == name).CameraState)
                                    {
                                        Console.Write($"{name} ");
                                        matches++;
                                    }
                                }
                                if (matches == 0) Console.Write("Никого");
                                Console.WriteLine();
                                //foreach (string resident in guiltyResidents) Console.WriteLine("Виновен" + resident); //
                                while (wrongNames)
                                {
                                    Console.Write($"Напишите минимум одно, а максимум - {guiltyNumber} имени тех, кого считаете виновным. Формат ввода: Имя1 Имя2 Имя3 и т.д: ");
                                    string namesString = Console.ReadLine();
                                    if (namesString != "") guessedGuiltyResidents = namesString.Split(' ').ToList();
                                    reportsNumber = guessedGuiltyResidents.Count;
                                    if (reportsNumber > guiltyNumber)
                                    {
                                        Console.WriteLine("Вы написали слишком много имен!");
                                        continue;
                                    }
                                    //foreach (string asd in guessedGuiltyResidents) Console.WriteLine(asd);
                                    foreach (string resident in guessedGuiltyResidents)
                                    {
                                        if (!residentNames.Contains(resident))
                                        {
                                            Console.WriteLine($"Жителя с именем \"{resident}\" нет в Вашем доме, попробуйте еще раз!");
                                            wrongNames = true;
                                            break;
                                        }
                                        else wrongNames = false;
                                    }
                                }
                                foreach (string r in guessedGuiltyResidents)
                                {
                                    residents.Find(x => x.Name == r).AddCrime();
                                    if (guiltyResidents.Contains(r)) countForPercent++;
                                }
                                guiltyByThisTime += guiltyNumber;
                                currentPercent = countForPercent * 100 / guiltyByThisTime;
                                //Console.WriteLine("%:" + currentPercent);
                                //foreach (Resident resident1 in residents) Console.WriteLine(resident1.Name + resident1.NumberOfReportedCrimes);
                                if (residentNames.Contains("Роберт") && guessedGuiltyResidents.Contains("Роберт") && info.Week >= 5)
                                {
                                    residents.Remove(residents.Find(x => x.Name == "Роберт"));
                                    info.AddTime(3);
                                    Console.Write("Резидент Роберт был арестован, так как был под подозрением партии, а Ваш отчет подтвердил его подрывную деятельность. Нажмите любую клавишу для продолжения");
                                    Console.ReadKey();
                                    break;
                                }
                                Console.Write($"Отчет был отправлен. За него Вам заплатили {reportsNumber * 40} брум. Нажмите любую клавишу для продолжения");
                                Console.ReadKey();
                                info.AddTime(3);
                                info.AddMoney(reportsNumber * 40);
                            }
                            break;
                        }
                    case "18": //переписать с провекрой функции на бессоную ночь (влом)
                        {
                            border = 4;
                            if (info.Time > border)
                            {
                                Console.CursorVisible = false;
                                Console.WriteLine("Слишком поздно идти на подработку! Нажмите любую клавишу для продолжения.");
                                Console.ReadKey();
                                Console.CursorVisible = true;
                                break;
                            }
                            if (info.Time == border - 1)
                            {
                                while (true)
                                {
                                    Console.Write("Вы не будете спать ночь, Вы уверены, что хотите сделать это? Напишите \"Да\" или \"Нет\": ");
                                    string str = Console.ReadLine().ToLower();
                                    if (str == "да")
                                    {
                                        info.ChangeStress(15);
                                        info.AddDebuff();
                                        info.AddTime(3);
                                        info.AddMoney(50);
                                        Console.CursorVisible = false;
                                        Console.WriteLine("Вы заработили 50 брум. Ваш стресс повышен. Нажмите любую клавишу для продолжения.");
                                        Console.ReadKey();
                                        Console.CursorVisible = true;
                                        break;
                                    }
                                    else if (str == "нет")
                                    {
                                        Console.CursorVisible = false;
                                        Console.WriteLine("Вы не пойдете на подработку. Нажмите любую клавишу для продолжения.");
                                        Console.ReadKey();
                                        Console.CursorVisible = true;
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Формат ввода ответа был неверный, попробуйте еще раз!");
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                info.AddMoney(50);
                                Console.CursorVisible = false;
                                info.AddTime(3);
                                Console.Write("Вы заработали 50 брум. Нажмите любую клавишу для продлжения");
                                Console.ReadKey();
                                Console.CursorVisible = true;
                                break;
                            }
                            break;
                        }
                    case "19":
                        {
                            info.ShowJournal(false);
                            break;
                        }
                    case "20":
                        {
                            Console.Write("Напишите, что Вы хотите внести в журнал. Для отмены оставте строку пустой:  ");
                            string str = Console.ReadLine();
                            if (str == "") break;
                            else info.AddJournalEntry(str);
                            break;
                        }
                    case "21":
                        {
                            info.RemoveEntryFromJournal();
                            break;
                        }
                    case "22":
                        {
                            info.AddTime(4);
                            break;
                        }
                    case "23":
                        {
                            info.ChangeTime();
                            break;
                        }
                    case "24":
                        {
                            break;
                        }
                    case "25":
                        {
                            while (true)
                            {
                                Console.Write("Какой кубик Вам нужно бросить? Введите максимально возможный результат на кубике или \"0\" для того, чтобы выйти: ");
                                string str = Console.ReadLine();
                                if (str == "0") break;
                                if (int.TryParse(str, out int value))
                                {
                                    DiceRoller(value);
                                    Console.WriteLine("\nНажмите любую клавишу для продолжения");
                                    Console.ReadKey();
                                    break;
                                }
                                else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                            }
                            break;
                        }
                    case "26":
                        {
                            info.TakePills(ref pillsThisWeek);
                            break;
                        }
                    case "27":
                        {
                            Console.Write("Введить название сохранения: ");
                            SaveGame(Console.ReadLine() + ".txt", info, residents, guessedGuiltyResidents, itemsToExchangeThisWeek, paidDebt, notIdealInventory, firstTimeLootingRobert,
                        knowChristofer, oneTimeMessage, knowReginald, stressCounter, stealCounter, ignoredProblems, SophiaVisits, LouiseVisits, countForPercent, guiltyByThisTime,
                        wrongOccusations, exchangedItems, soldJunk, pillsThisWeek, currentPercent);
                            break;
                        }
                    default:
                        {
                            Console.CursorVisible = false;
                            Console.WriteLine("Такой команды нет, попробуйте еще раз! Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            Console.CursorVisible = true;
                            break;
                        }
                }
                if (info.Time > 4)
                {
                    info.AddProviant(-1);
                    if (info.Proviant <= 0)
                    {
                        info.AddDebuff(0.2);
                        info.ChangeStress(25);
                    }
                    info.SetTime(1);
                    if (info.NextDay(eventOccured1, info, ref itemsToExchangeThisWeek))
                    {
                        if (pillsThisWeek == 0) info.AddDebuff(1.5);
                        pillsThisWeek = 0;
                        if (stressCounter > 3) stressCounter -= 2;
                        if (stressCounter < 3) stressCounter = 3;
                        storeStock = store.RefreshStock(info);
                        Console.Clear();
                        Console.CursorVisible = false;
                        if (paidDebt == false)
                        {
                            Console.WriteLine("Так как вы не внесли платеж на этой неделе, Ваш долг был увеличен на 50 брум");
                            info.PayDebt(-50, false);
                        }
                        else paidDebt = false;
                        if (info.Week % 2 == 1 && info.Week != 9)
                        //if (info.Week == 2) //
                        {
                            //info.ChangeWeek(5); //
                            Console.WriteLine("Наступила новая неделя! На этой неделе приходит проверка: ");
                            if (stealCounter > 8)
                            {
                                Console.Write("По результатам проверки\n");
                                Console.Write("В Вашем доме участились случаи кражи. Просьба проследить за этим, так как жильцы недовольны. Нажмите любую клавишу для продолжения");
                            }
                            else if (stealCounter > 15)
                            {
                                Console.Write("По результатам проверки\n");
                                Console.Write("Кражи не прекратились и жильцы сильно обеспокоены этим. Просба приложить больше усилий, чтобы прекратить кражи.Нажмите любую клавишу для продолжения");
                            }
                            else if (stealCounter > 23)
                            {
                                Console.Write("По результатам проверки\n");
                                Console.Write("Жильцы подозревают Вас в кражах. Это последнее предупреждение. Нажмите любую клавишу для продолжения");
                            }
                            else if (stealCounter > 28)
                            {
                                Console.Write("По результатам проверки\n");
                                Console.Write("Вы крали слишком много и власти обнаружили это. Перейдите к концовке. Нажмите любую клавишу для продолжения");
                                Console.ReadKey();
                                break;
                            }
                            List<Resident> checkedResidents = new List<Resident>();
                            List<string> illegalItems = info.possibleItems.Where(x => x.Value == true).Select(x => x.Key).ToList();
                            if (info.Week == 7) foreach (Resident resident in residents) checkedResidents.Add(resident);
                            else checkedResidents = GenerateResidents(rand.Next(2, 5), residents);
                            //foreach(Resident res in checkedResidents) Console.WriteLine(res.Name); //

                            bool notIdealWeek = false;
                            foreach (Resident res in checkedResidents)
                            {
                                if (res.ItemsInRoom.ContainsValue(true)) //если есть запрещенный предмет 
                                {
                                    if (guessedGuiltyResidents.Contains(res.Name)) continue; //если его репортили то смотри следующего
                                    else //если не репортили, то инфа была ложная
                                    {
                                        wrongOccusations++;
                                        notIdealWeek = true;
                                    }   
                                }
                            }
                            if (info.Week == 7)
                            {
                                foreach (string item in info.inventory)
                                {
                                    if (illegalItems.Contains(item)) notIdealInventory = true; break;
                                }
                            }
                            if (notIdealInventory)
                            {
                                Console.WriteLine("Проверка обнаружила незаконные предметы в Вашем инвентаре! Просьба избавиться от них.");
                            }
                            if (notIdealWeek)
                            {
                                Console.Write("Проверка обнаружила несоответствия в Ваших отчетах и действительности. Впредь будте внимательнее. Нажмите любую клавишу для продолжения");
                                notIdealWeek = false;
                            }
                        }
                        else Console.Write("Наступила новая неделя! Нажмите любую клавишу для продолжения");
                        foreach (Resident resident in residents) resident.EndWeekItems(rand);
                        Console.ReadKey();
                        Console.CursorVisible = true;
                        SaveGame($"[Autosave] Week {info.Week - 1} End.txt", info, residents, guessedGuiltyResidents, itemsToExchangeThisWeek, paidDebt, notIdealInventory, firstTimeLootingRobert,
                        knowChristofer, oneTimeMessage, knowReginald, stressCounter, stealCounter, ignoredProblems, SophiaVisits, LouiseVisits, countForPercent, guiltyByThisTime,
                        wrongOccusations, exchangedItems, soldJunk, pillsThisWeek, currentPercent);
                    }
                    else
                    {
                        Console.Clear();
                        Console.CursorVisible = false;
                        Console.WriteLine("Наступил новый день! Нажмите любую клавишу для продолжения.");
                        Console.ReadKey();
                        Console.CursorVisible = true;
                    }
                    info.CheckForBreakdown(ref stressCounter, eventOccured1, info, ref itemsToExchangeThisWeek);
                    guessedGuiltyResidents.Clear();
                    //itemsToExchangeThisWeek.Clear();
                    foreach (Resident res in residents)
                    {
                        if (res.NumberOfReportedCrimes > 5)
                        {
                            Console.Write($"Резидент под именем {res.Name} был(а) выселен(а) из-за большого количества сообщенных преступлений. Нажмите любую клавишу для продолжения");
                            residents.Remove(res);
                            Console.ReadKey();
                            break;
                        }
                    }
                    if (residents.Count == 0)
                    {
                        Console.Write("В вашем доме не осталось жителей. Перейдите к концовке. Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        break;
                    }
                    if (exchangedItems >= 4)
                    {
                        Console.Write("Вам пришло письмо от сопротивления. Ведущий должен его зачитать. Как будете готовы, нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        continue;
                    }
                }
            }
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Игра окончена! Сейчас ведущий огласит концовку. Для этого, ему нужна следующая информация:");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"\t*Долг = {info.Debt}\n\t*Debuff = {info.Debuff}\n\t*ignoredProblems = {ignoredProblems}\n\t*SophiaVisits = {SophiaVisits}\n\t*LouiseVisits = {LouiseVisits}" +
                $"\n\t*percent = {currentPercent}\n\t*notIdealInventory = {notIdealInventory}\n\t*wrongOccusations = {wrongOccusations}\n\t*stealCounter = {stealCounter}\n\t*residents.Count() = {residents.Count()}"); 
            while (true)
            {
                Console.Write("После получения концовки введите \"GGWP\": ");
                if (Console.ReadLine().ToLower() == "ggwp") break;
            }
            //endgame segment - debt check, show debuff, ignoredProblems, SophiaVisits LouiseVisits, percent - процент правильных досье notIdealInventory exchangedItems wrongOccusations 
        }
        enum DayOfTheWeek
        {
            ПН,
            ВТ,
            СР,
            ЧТ,
            ПТ,
            СБ,
            ВС
        }
        enum Events
        {
            Морозы,
            Электричество,
            Газ,
            Забастовка,
            Задымление,
            Пожар,
            Затопление,
            Обрушение,
            Подрыв,
            Драка,
            Ничего = 100
        }
        class Information
        {
            public int Money { get; private set; }
            public int Debt { get; private set; }
            public int Stress { get; private set; }
            public int Proviant { get; private set; }
            public int Time { get; private set; }
            public DayOfTheWeek Day { get; private set; }
            public int Week {  get; private set; }
            public double Debuff { get; private set; }
            public int WeeksWithoutEvents { get; private set; }
            public Events lastEvent { get; set; }
            public List<Events> happenedEvents { get; private set; }
            public Dictionary<string, bool> possibleItems = new Dictionary<string, bool> //false - legal; true - illegal
            {
                { "Камера", false },
                { "Прослушка", false },
                { "Пиво", false },
                { "Виски", false },
                { "Наркотики", true },
                { "Пластинка(запрещ)", true },
                { "Пластинка(легал)", false },
                { "Видеокассета(запрещ)", true },
                { "Видеокассета(легал)", false },
                { "Оружие", true },
                { "Диск(запрещ)", true },
                { "Досье", false },
                { "Журнал", false },
                { "Цветы", false },
                { "Лекарства", false },
                { "Лекарства(имп)", false },
                { "Ваза", false },
                { "Сервиз", false },
                { "Сундучок", false },
                { "Провизия", false },
                { "Телевизор", false },
                { "Суп(5пров)", false },
            };
            public List<string> inventory = new List<string>();
            public List<string> journal = new List<string>();
            static bool inSimulation = false;
            static string eventsOccuredInSimulation = "";


            public Information()
            {
                Money = 50;
                Debt = -2000;
                Stress = 0;
                Proviant = 5;
                Time = 1;
                Day = (DayOfTheWeek)0;
                Week = 1;
                Debuff = 10;
                lastEvent = (Events)100;
            }
            public Information(int money, int debt, int stress, int proviant, int time, int dayIndex, int week, double debuff, int lastEventIndex) 
            {
                Money = money;
                Debt = debt;
                Stress = stress;
                Proviant = proviant;
                Time = time;
                Day = (DayOfTheWeek)dayIndex;
                Week = week;
                Debuff = debuff;
                lastEvent = (Events)lastEventIndex;
            }
            public Information(string str)
            {
                List<string> parameters = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Money = Convert.ToInt32(parameters[0]);
                Debt = Convert.ToInt32(parameters[1]);
                Stress = Convert.ToInt32(parameters[2]);
                Proviant = Convert.ToInt32(parameters[3]);
                Time = Convert.ToInt32(parameters[4]);
                Day = (DayOfTheWeek)Convert.ToInt32(parameters[5]);
                Week = Convert.ToInt32(parameters[6]);
                Debuff = Convert.ToDouble(parameters[7]);
                lastEvent = (Events)Convert.ToInt32(parameters[8]);
            }
            public void ShowInfo()
            {
                Console.WriteLine($"Деньги: {Money}\t\tДолг: {Debt}\t\tСтресс: {Stress}/100\t\tПровизия: {Proviant}\nЧасть дня: {Time}\t\tДень недели: {Day}\t\tНеделя: {Week}");
                Console.WriteLine(new string('-', 85));
                ShowInventory();
            }
            public void ShowInventory()
            {
                Console.Write("Ваш инвентарь:\n\t|");
                int count = 0;
                if (inventory.Count == 0) Console.WriteLine("Предметов нет");
                foreach (string item in inventory)
                {
                    Console.Write(item + "|");
                    if (count % 6 == 0 && count != 0) Console.Write("\n\t|");
                    count++;
                }
                Console.WriteLine();
            }
            public void TakePills(ref double pillsThisWeek)
            {
                double pillValue = 0.5f;
                double bestPillValue = 1.25f;
                if (pillsThisWeek > 0)
                {
                    pillValue -= pillsThisWeek * 0.35;
                    bestPillValue -= pillsThisWeek * 0.55;
                }
                if (inventory.Contains("Лекарства") && inventory.Contains("Лекарства(имп)"))
                {
                    while(true)
                    {
                        Console.Write("У Вас в инвентаре есть 2 вида лекарств, какое Вы хотите принять? Напишите название с большой буквы (если ошиблись, оставте строку пустой): ");
                        string str = Console.ReadLine();
                        if (str == "")
                        {
                            Console.Write("Вы решили не принемать лекарства. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                        if (inventory.Contains(str))
                        {
                            inventory.Remove(str);
                            if (str == "Лекарства") AddDebuff(-pillValue);
                            if (str == "Лекарства(имп)") AddDebuff(-bestPillValue);
                            pillsThisWeek++;
                            Console.Write($"Вы приняли {str}. Пожалуйста, воздержитесь от приема таблеток на этой неделе.");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Вы ввели несуществующее название лекарств, попробуйте еще раз!");
                            continue;
                        }
                    }
                }
                else if (inventory.Contains("Лекарства"))
                {
                    while (true)
                    {
                        Console.Write("У Вас в инвентаре есть Лекарства. Согласны ли Вы их выпить? Напишите \"Да\" или \"Нет\": ");
                        string str = Console.ReadLine();
                        if (str == "Нет")
                        {
                            Console.Write("Вы решили не принемать лекарства. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                        else if (str == "Да") str = "Лекарства";
                        else
                        {
                            Console.WriteLine("Вы ввели что-то не то, попробуйте еще раз!");
                            continue;
                        }
                        inventory.Remove(str);
                        AddDebuff(-pillValue);
                        pillsThisWeek++;
                        Console.Write($"Вы приняли {str}. Пожалуйста, воздержитесь от приема таблеток на этой неделе.");
                        Console.ReadKey();
                        break;
                    }
                }
                else if (inventory.Contains("Лекарства(имп)"))
                {
                    while (true)
                    {
                        Console.Write("У Вас в инвентаре есть Лекарства(имп). Согласны ли Вы их выпить? Напишите \"Да\" или \"Нет\": ");
                        string str = Console.ReadLine();
                        if (str == "Нет")
                        {
                            Console.Write("Вы решили не принемать Лекарства(имп). Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                        else if (str == "Да") str = "Лекарства(имп)";
                        else
                        {
                            Console.WriteLine("Вы ввели что-то не то, попробуйте еще раз!");
                            continue;
                        }
                        inventory.Remove(str);
                        AddDebuff(-bestPillValue);
                        pillsThisWeek++;
                        Console.Write($"Вы приняли {str}. Пожалуйста, воздержитесь от приема таблеток на этой неделе.");
                        Console.ReadKey();
                        break;
                    }
                }
                else
                {
                    Console.Write("У Вас в инвентаре нет лекарств! Нажмите любую клавишу для продолжения");
                    Console.ReadKey();
                }
            }
            public void AddJournalEntry(string str)
            {
                journal.Add(str);
            }
            public void ShowJournal(bool fromRemove)
            {
                Console.WriteLine("Записи в журнале:");
                if (journal.Count != 0)
                {
                    for (int i = 0; i < journal.Count; i++)
                    {
                        Console.WriteLine($"\t{i + 1}) {journal[i]}");
                    }
                }
                else Console.WriteLine("Записи отсутствуют!");
                if (!fromRemove)
                {
                    Console.Write("Нажмите любую клавишу, чтобы закрыть журнал");
                    Console.ReadKey();
                }
            }
            public void RemoveEntryFromJournal()
            {
                ShowJournal(true);
                while (true)
                {
                    Console.Write("Какую запись Вы хотите удалить из журнала? Если не хотите удалять запись, то введите 0: ");
                    string str = Console.ReadLine();
                    if (Convert.ToInt32(str) == 0) break;
                    if (Convert.ToInt32(str) > journal.Count)
                    {
                        Console.Write("Такой записи нет в журнале. Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        break;
                    }
                    if (int.TryParse(str, out int value))
                    {
                        journal.RemoveAt(value - 1);
                        Console.Write("Запись успешно удалена. Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        break;
                    }
                    else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                }
            }
            public void ChangeTime()
            {
                while (true)
                {
                    Console.Write($"На какую величину изменить время? Текущая часть дня: {Time}. Введите число: ");
                    if (int.TryParse(Console.ReadLine(), out int value))
                    {
                        if (Time + value > 0 && Time + value < 5)
                        {
                            Time += value;
                            Console.Write($"Успешно! Сейчас {Time} часть дня. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            Console.Write("Такую величину добавить невозможно. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                    }
                    else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                }
            }
            public void AddItem()
            {
                string str = "";
                for(int i = 0; i < possibleItems.Count; i++)
                {
                    if (str == "")
                    {
                        Console.Write("Напишите название предмета, который хотите добавить (с большой буквы). Если передумали, введите \"0\": ");
                        str = Console.ReadLine();
                        if (str == "0") break;
                        i = 0;
                    }
                    if (str == possibleItems.ElementAt(i).Key)
                    {
                        inventory.Add(str);
                        inventory.Sort();
                        Console.Write("Предмет был добавлен, нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        break;
                    }
                    if (i == possibleItems.Count - 1)
                    {
                        Console.Write("Такого предмета нет в игре! Попробуйте еще раз.");
                        i = -1;
                        str = "";
                        Console.WriteLine();
                    }
                }
            }
            public void AddItem(string item)
            {
                inventory.Add(item);
            }
            public void RemoveItem() 
            {
                string str = "";
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (str == "")
                    {
                        Console.Write("Напишите название предмета, который хотите удалить (с большой буквы). Если передумали, введите \"0\": ");
                        str = Console.ReadLine();
                        if (str == "0") break;
                        i = 0;
                    }
                    if (str == inventory[i])
                    {
                        inventory.RemoveAt(i);
                        inventory.Sort();
                        Console.Write("Предмет был удален, нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        break;
                    }
                    if (i == inventory.Count - 1)
                    {
                        Console.Write("Такого предмета нет в инвентаре! Попробуйте еще раз.");
                        i = -1;
                        str = "";
                        Console.WriteLine();
                    }
                }
            }
            public void RemoveItem(string item)
            {
                inventory.Remove(item);
                inventory.Sort();
            }
            public void AddMoney(int additionalMoney)
            {
                if (Money + additionalMoney >= 0) Money += additionalMoney;
                //else if (Money + additionalMoney < 0 && additionalMoney < 0) Money = 0;
                else
                {
                    Console.Write("У вас не хватило денег! Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                }
            }
            public bool AddMoneyBool(int additionalMoney)
            {
                if (Money + additionalMoney >= 0)
                {
                    Money += additionalMoney;
                    return true;
                }
                else
                {
                    Console.Write("У вас не хватило денег! Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                    return false;
                }
            }
            public void PayDebt(int amount, bool extraInfo)
            {
                if (amount > Money)
                {
                    Console.Write("Вам не хватает денег для оплаты долга. Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                }
                else
                {
                    Debt += amount;
                    if (amount >= 0) Money -= amount;
                    if (extraInfo) Console.Write($"Вы заплатили {amount} брум. Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                }
            }
            public void ChangeStress(int amount) 
            { 
                Stress += amount;
                if (Stress >= 100) Stress = 100;
                if (Stress < 0) Stress = 0;
            }
            public void AddTime (int amount = 1)
            {
                Time += amount;
            }
            public void SetTime(int value)
            {
                Time = value;
            }
            public bool NextDay(bool eventOccured, Information info, ref List<string> itemsToExchangeThisWeek) 
            {
                if (CheckForEvents(ref eventOccured))
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    lastEvent = GiveRandomEvent();
                    if (inSimulation) eventsOccuredInSimulation += lastEvent + ", ";
                    Console.WriteLine($"Событие: {lastEvent}. У Вас день для решения проблемы.");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ReadKey();
                }
                
                if ((int)Day < 6) Day++;
                else
                {
                    Day = 0;
                    NextWeek(ref eventOccured, info, ref itemsToExchangeThisWeek);
                    return true;
                }
                return false;
            }
            public void NextWeek(ref bool eventOccured, Information info, ref List<string> itemsToExchangeThisWeek)
            {
                info.ChangeStress(4 + Week);
                Week++;
                itemsToExchangeThisWeek.Clear();
                Random rand = new Random();
                List<string> illegalItems = info.possibleItems.Where(x => x.Value == true).Select(x => x.Key).ToList();
                int index = rand.Next(0, illegalItems.Count);
                itemsToExchangeThisWeek.Add(illegalItems[index]);
                illegalItems.RemoveAt(index);
                index = rand.Next(0, illegalItems.Count);
                itemsToExchangeThisWeek.Add(illegalItems[index]);
                if (eventOccured) eventOccured = false;
                else WeeksWithoutEvents++;
            }
            public void ChangeWeek(int value)
            {
                Week += value;
            }
            public void AddDebuff(double value = 1.0)
            {
                Debuff += value;
            }
            public void AddProviant(int amount = 1)
            {
                Proviant += amount;
                if (Proviant < 0) Proviant = 0;
            }
            public void CheckForBreakdown(ref int checkNumber, bool eventOccured, Information info, ref List<string> itemsToExchangeThisWeek)
            {
                if (Stress >= 100)
                {
                    Console.WriteLine();
                    Random rand = new Random();
                    Console.Write($"Происходит проверка на нервный срыв. Текущий порог d20 <= {checkNumber}. Нажмите любую кнопку, чтобы бросить кость.");
                    Console.ReadKey();
                    int result = DiceRoller(20);
                    if (result <= checkNumber)
                    {
                        Console.WriteLine("Проверка провалена, произошел нервный срыв");
                        Stress = 0;
                        checkNumber -= 6;
                        if (checkNumber < 3) checkNumber = 3;
                        Debuff += 1.65;
                        Simulation(rand.Next(1, 9), eventOccured, info, ref itemsToExchangeThisWeek);
                        //Simulation(100, eventOccured);
                        Console.Write($"Когда Вы пришли в себя, то с удивлением обнаружили, что сейчас {Day}, неделя {Week}. Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        //Console.WriteLine($"stressCounter = {checkNumber}");
                        //Console.ReadKey();
                    }
                    else
                    {
                        Console.Write("Вы смогли сдержать нервы. Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        ChangeStress(-40);
                        checkNumber += 4;
                        Debuff += 0.3;
                        //Console.WriteLine($"stressCounter = {checkNumber}");
                        //Console.ReadKey();
                    }
                }
                
            }
            public void Simulation (int time, bool eventOccured, Information info, ref List<string> itemsToExchangeThisWeek)
            {
                inSimulation = true;
                while (true)
                {
                    time--;
                    AddTime(1);
                    if (Time == 4)
                    {
                        NextDay(eventOccured, info, ref itemsToExchangeThisWeek);
                        SetTime(1);
                        if (Proviant > 0) AddProviant(-1);
                    }
                    if (time == 0) break;
                }
                //if (eventsOccuredInSimulation != "") Console.WriteLine($"За это время произошли: {eventsOccuredInSimulation}");
                inSimulation = false;
                eventsOccuredInSimulation = "";
            }
            public bool CheckForEvents(ref bool eventOccured)
            {
                Random rand = new Random();
                double probability = (Week * Week * 0.5 + WeeksWithoutEvents * WeeksWithoutEvents * 2.5 + 12.5) * Convert.ToInt32(!eventOccured);
                if (rand.Next(1, 101) <= probability)
                {
                    //Console.WriteLine(probability);
                    eventOccured = true;
                    WeeksWithoutEvents = 0;
                    //Console.BackgroundColor = ConsoleColor.Red;
                    //lastEvent = GiveRandomEvent();
                    //Console.WriteLine($"Событие: {lastEvent}");
                    //Console.BackgroundColor = ConsoleColor.Black;
                    //Console.ReadKey();
                    return true;
                }
                else return false;

            }
            public Events GiveRandomEvent()
            {
                Random rand = new Random();
                Dictionary<Events, int> eventsWithProbability = new Dictionary<Events, int>
                {

                    { Events.Драка, 0 },
                    { Events.Электричество, 21},
                    { Events.Морозы, 41},
                    { Events.Газ, 51},
                    { Events.Затопление, 61},
                    { Events.Задымление, 71},
                    { Events.Пожар, 81},
                    { Events.Забастовка, 89},
                    { Events.Обрушение, 95},
                    { Events.Подрыв, 98}

                };
                int i = rand.Next(1, 101);
                int j = 0;
                foreach(int value in eventsWithProbability.Values)
                {
                    //if (i >= 99) return eventsWithProbability.Last().Key;
                    //Console.WriteLine(i);
                    if (i - value <= 0) return eventsWithProbability.ElementAt(j - 1).Key;
                    j++;
                }
                return eventsWithProbability.Last().Key;


            }
        }
        class Resident : Information
        {
            public string Name { get; private set; }
            public int Room { get; private set; }
            public string TimeAtHome { get; private set; }
            public bool KnowInfo { get; private set; }
            public int NumberOfReportedCrimes {  get; private set; }
            public Dictionary<string, bool> ItemsInRoom = new Dictionary<string, bool>();
            public bool CameraState {  get; private set; }
            public Resident(string name, int room, string timeAtHome, int crimes, Dictionary<string, bool> itemsInRoom)
            {
                Name = name;
                Room = room;
                TimeAtHome = timeAtHome;
                NumberOfReportedCrimes = crimes;
                KnowInfo = false;
                ItemsInRoom = itemsInRoom;
                CameraState = false;
            }
            public void ShowResident(Information info)
            {
                if (KnowInfo)
                {
                    int[] array = TimeAtHome.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{Name} из комнаты {Room}:");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Бывает дома: ");
                    for (int i = 0; i <= (int)DayOfTheWeek.ВС; i++)
                    {
                        if (info.Day == (DayOfTheWeek)i) Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\t{(DayOfTheWeek)i}: {array[i]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else Console.WriteLine($"У вас нет информации, когда {Name} дома.");
            }
            public void ShowResident(Information info, bool youKnow)
            {
                if (youKnow)
                {
                    int[] array = TimeAtHome.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{Name} из комнаты {Room}:");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Бывает дома: ");
                    for (int i = 0; i <= (int)DayOfTheWeek.ВС; i++)
                    {
                        if (info.Day == (DayOfTheWeek)i) Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\t{(DayOfTheWeek)i}: {array[i]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else Console.WriteLine($"У вас нет информации, когда {Name} дома.");
            }
            public void ChangeStatus()
            {
                KnowInfo = true;
            }
            public void ChangeCameraState()
            {
                CameraState = true;
            }
            public bool IsResidentHome(int time, DayOfTheWeek day)
            {
                if ( TimeAtHome.Split(',')[(int)day].Contains(Convert.ToString(time)) ) return true;
                else return false;
            }
            public List<string> GiveItemsInRoom(List<Resident> residents)
            {
                List<string> list = new List<string>();
                //Resident res = residents.Find(x => x.Name == Name);
                foreach (string item in ItemsInRoom.Keys) { list.Add(item); }
                return list;
            }
            public string ShowItem(int i)
            {
                return ItemsInRoom.ElementAt(i).Key.ToString();
            }
            public void Loot()
            {
                Console.WriteLine($"{Name} в своей квартире хранит следующие интересные вещи:");
                if (ItemsInRoom.Count == 0) Console.WriteLine("Комната пуста!");
                for(int i = 0; i < ItemsInRoom.Count; i++)
                {
                    string str = ShowItem(i);
                    if (str == "Наркотики" || str == "Оружие" || str == "Видеокассета(запрещ)" || str == "Диск(запрещ)" || str == "Пластинка(запрещ)") Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\t{i + 1}) " + str);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            public string Steal(ref int stealCounter) //не дает предмет в инвентарь игрока - добавляю функцию AddItem
            {
                while (true)
                {
                    Console.Write("Если Вы хотите украсть предмет, то введите номер предмета. Если же Вы не хотите красть ничего, то введите \"0\": ");
                    if (int.TryParse(Console.ReadLine(), out int value) && value <= ItemsInRoom.Count)
                    {
                        if (value == 0) break;
                        string stolenItem = ItemsInRoom.ElementAt(value - 1).Key;
                        if (stolenItem.Contains("брум"))
                        {
                            stealCounter += Convert.ToInt32(stolenItem.Split(' ')[0]) / 50;
                        }
                        ItemsInRoom.Remove(stolenItem);
                        stealCounter++;
                        Console.WriteLine("Оставшиеся предметы в комнате: ");
                        if (ItemsInRoom.Count == 0) Console.WriteLine("Комната пуста!");
                        for (int i = 0; i < ItemsInRoom.Count; i++)
                        {
                            Console.WriteLine($"\t{i + 1}) " + ShowItem(i));
                        }
                        //Console.WriteLine($"stealCounter = {stealCounter}");
                        Console.Write("Предмет был украден");
                        //Console.ReadKey();
                        Console.WriteLine();
                        return stolenItem;
                    }
                    else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                }
                return "";
            }
            public void AddLootToRoom(Information info, ref int stealCounter) //Предметы в комнатах уникальны 
            {
                bool itemAdded = false;
                bool isIllegal = false;
                string itemFromInventory = "";
                //foreach (string item in info.inventory) Console.WriteLine("Инвентарь " + item);
                while (!itemAdded)
                {
                    Console.Write("Вы можете:\n\t1) Добавить предмет из инвентаря\n\t2) Добавить деньги\n\t3) Уйти\nВведите номер желаемого действия: ");
                    if (int.TryParse(Console.ReadLine(), out int value))
                    {
                        switch (value)
                        {
                            case 1:
                                {
                                    Console.Write("Какой предмет Вы хотите добавить? Введите название. Если Вы передумали, оставте строчку пустой: ");
                                    itemFromInventory = Console.ReadLine();
                                    foreach (var item in ItemsInRoom)
                                    {
                                        if (item.Key == itemFromInventory)
                                        {
                                            Console.WriteLine("Этот предмет уже есть в комнате, Вы не можете его добавить.");
                                            itemFromInventory = "";
                                            break;
                                        }
                                    }
                                    
                                    if (itemFromInventory == "") break;
                                    if (info.inventory.Contains(itemFromInventory))
                                    {
                                        itemAdded = true;
                                        info.inventory.Remove(itemFromInventory);
                                        string[] keys = possibleItems.Keys.ToArray();
                                        int index = 0;
                                        foreach (string key in keys)
                                        {
                                            if (key == itemFromInventory) break;
                                            index++;
                                        }
                                        isIllegal = possibleItems.ElementAt(index).Value;
                                        ItemsInRoom.Add(itemFromInventory, isIllegal);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Такого предмета нет в Вашем инвентаре, попробуйте еще раз!");
                                        break;
                                    }
                                    break;
                                }
                            case 2: //дописать отсюда
                                {
                                    while (true)
                                    {
                                        Console.Write("Введите сумму (только число), которую хотите добавить. Если ошиблись - введите \"0\":  ");
                                        string moneyAmount = Console.ReadLine();
                                        bool moneyAdded = false;
                                        //if (moneyAmount == "") break;
                                        if ( (int.TryParse(moneyAmount, out int sum)) && (sum >= 0) && (Money - sum >= 0) )
                                        {
                                            //sum = sum * (-1);
                                            if (sum == 0) break;
                                            info.AddMoney(-sum);
                                            //Console.WriteLine(Money);
                                            string[] items = ItemsInRoom.Keys.ToArray();
                                            foreach (string item in items)
                                            {
                                                if (item == $"{sum} брум")
                                                {
                                                    ItemsInRoom.Remove($"{sum} брум");
                                                    ItemsInRoom.Add($"{sum * 2} брум", false);
                                                    moneyAdded = true;
                                                }
                                            }
                                            if (!moneyAdded) ItemsInRoom.Add($"{sum} брум", false);
                                            itemAdded = true;
                                            stealCounter--;
                                            break;
                                        }
                                        else if (sum < 0) Console.WriteLine("Вы не можете добавить отрицательное количество денег. Попробуйте еще раз");
                                        else if (Money - sum < 0) Console.WriteLine("Вам не хватает денег! Введите другую сумму");
                                        else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    itemAdded = true;
                                    Console.WriteLine("Вы решили уйти из комнаты.");
                                    //Console.ReadKey();
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("Такой команды нет, попробуйте еще раз!");
                                    break;
                                }
                        }
                    }
                    else Console.WriteLine("Формат ввода был неверен, попробуйте еще раз!");
                }       
            }
            public void PlaceCamera(Information info, ref bool restart)
            {
                if (!CameraState && info.inventory.Contains("Камера"))
                {
                    CameraState = true;
                    info.inventory.Remove("Камера");
                    Console.WriteLine("Камера была установлена успешно.");
                }
                else if (!info.inventory.Contains("Камера"))
                {
                    Console.WriteLine("У Вас нет камеры в инвентаре!");
                    restart = true;
                }
                else
                {
                    Console.WriteLine("В этой комнате уже есть камера!");
                    restart = true;
                }
            }
            public void FillGuiltyResidents(ref List<string> guiltyResidents)
            {
                //Console.WriteLine($"Проверяю {Name}");
                if (ItemsInRoom.Values.Select(x => Convert.ToInt32(x)).Sum() > 0 && Name != "София")
                {
                    guiltyResidents.Add(Name);
                }
            }
            public void AddCrime()
            {
                NumberOfReportedCrimes++;
                //Console.WriteLine($"{Name}: {NumberOfReportedCrimes}");
            }
            public void EndWeekItems(Random rand)
            {
                int index = 0;
                while (true)
                {
                    index = rand.Next(0, possibleItems.Count);
                    if (ItemsInRoom.ContainsKey(possibleItems.ElementAt(index).Key)) continue;
                    else
                    {
                        if (ItemsInRoom.Count >= 10) break;
                        if (Name == "София" && possibleItems.ElementAt(index).Value == false) 
                        {
                            ItemsInRoom.Add(possibleItems.ElementAt(index).Key, possibleItems.ElementAt(index).Value);
                            break;
                        }
                        if (Name != "София") ItemsInRoom.Add(possibleItems.ElementAt(index).Key, possibleItems.ElementAt(index).Value);
                        break;
                    }
                }
            }
            public void EndWeekItems()
            {
                int i = 0;
                int itterations = 0;
                Random rand = new Random();
                while (true)
                {
                    if (ItemsInRoom.Count >= 9) break;
                    i = rand.Next(0, possibleItems.Count);
                    KeyValuePair<string, bool> item = possibleItems.ElementAt(i);
                    if (ItemsInRoom.ContainsKey(item.Key)) continue;
                    if (Name == "София" && item.Value == true) continue;
                    if (Name == "Барни" && !ItemsInRoom.Keys.Contains("Лекарства(имп)"))
                    {
                        ItemsInRoom.Add("Лекарства(имп)", false); 
                        break;
                    }
                    if (Name == "Юна" && !ItemsInRoom.Keys.Contains("Лекарства"))
                    {
                        ItemsInRoom.Add("Лекарства", false); 
                        break;
                    }
                    if (Name == "Кристофер" && !ItemsInRoom.Keys.Contains("Камера"))
                    {
                        ItemsInRoom.Add("Камера", false);
                        break;
                    }
                    if (itterations++ > 10000) break;
                }
            }
            public void SellOrExchange (ref Information info, ref int exchangedItems, List<string> itemsToExchangeThisWeek)
            {
                bool exit = true;
                bool continueEarly = false;
                List<string> illegalItems = info.possibleItems.Where(x => x.Value == true).Select(x => x.Key).ToList();
                string yourItem = itemsToExchangeThisWeek[0];
                string theirItem = itemsToExchangeThisWeek[1];
                while (true)
                {
                    continueEarly = false;
                    Console.Write("Вы можете:\n\t1) Обменять нелегальные предметы\n\t2) Продать нелегальные предметы по цене 25 брум за шт.\n\t3) Покинуть комнату\nВведите желаемое действие: ");
                    if (int.TryParse(Console.ReadLine(), out int value))
                    {
                        if (value == 3)
                        {
                            Console.WriteLine("Вы решили покинуть комнату Роберта. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            break;
                        }
                        if (value == 1)
                        {
                            Console.WriteLine($"Роберт предлагает обменять: Ваш предмет {yourItem} на его предмет {theirItem}");
                            if (info.inventory.Contains(yourItem))
                            {
                                Console.Write("У Вас есть нужный предмет. Если хотите обменять его, введите \"Да\". Если передумали, введите \"Нет\": ");
                                switch (Console.ReadLine())
                                {
                                    case "Да":
                                        {
                                            info.RemoveItem(yourItem);
                                            info.AddItem(theirItem);
                                            info.inventory.Sort();
                                            exchangedItems++;
                                            exit = true;
                                            Console.Write("Вы успешно обменялись с Робертом предметами. Если будет еще - приходите! Новая пара предметов на новой неделе. Нажмите любую клавишу для продолжения");
                                            Console.ReadKey();
                                            break;
                                        }
                                    case "Нет":
                                        {
                                            Console.WriteLine("Вы решили не менять предмет.");
                                            exit = false;
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Вы ввели что-то не то!");
                                            exit = false;
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.WriteLine("У Вас нет нужных предметов в инветаре!");
                                Console.BackgroundColor = ConsoleColor.Black;
                                continue;
                            }
                        }
                        if (value == 2)
                        {
                            int i = 1;
                            List<string> illegalItems2 = info.possibleItems.Where(x => x.Value == true).Select(x => x.Key).ToList();
                            List<string> illegalItemsInInventory = new List<string>();
                            Console.WriteLine("Вы можете продать следующие предметы из своего инвентаря: ");

                            foreach (string item in illegalItems2)
                            {
                                if (info.inventory.Contains(item))
                                {
                                    illegalItemsInInventory = info.inventory.FindAll(x => x == item).Concat(illegalItemsInInventory).ToList();
                                    //illegalItemsInInventory.Add(item);
                                    //Console.WriteLine($"\t{i}) {item}");
                                    i++;
                                }
                            }
                            if (i == 1)
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.WriteLine("В вашем инвентаре все предметы легальные!");
                                Console.BackgroundColor = ConsoleColor.Black;
                                exit = false;
                            }
                            else
                            {
                                char[] separator = new char[] { ' ' };
                                int sum = 0;
                                for (int n = 0; n < illegalItemsInInventory.Count; n++) Console.WriteLine($"\t{n + 1}) {illegalItemsInInventory[n]}");
                                Console.Write("Введите номера предметов, которые хотите продать, в формате Цифра1 Цифра2 Цифра3. Если передумали, введите \"0\": ");
                                string str = Console.ReadLine();
                                if (str == "0") continue;
                                List<string> indexesToSell = str.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
                                List<int> indexesToSellInt = new List<int>();
                                foreach (string s in indexesToSell)
                                {
                                    if (indexesToSell.FindAll(x => x == s).Count > 1)
                                    {
                                        Console.WriteLine("Вы ввели несколько одинаковых индексов, исправте ввод!");
                                        continueEarly = true;
                                        break;
                                    }
                                }
                                if (continueEarly) continue;
                                foreach (string item in indexesToSell)
                                {
                                    if (int.TryParse(item, out int value1) && value1 <= illegalItemsInInventory.Count)
                                    {
                                        indexesToSellInt.Add(value1);
                                        sum += 25;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Какое-то из введеных чисел имело не тот формат, попробуйте еще раз!");
                                        indexesToSellInt.Clear();
                                        exit = false;
                                        sum = 0;
                                        break;
                                    }
                                    //foreach (int asd in indexesToSellInt) Console.WriteLine($"item number: {asd}"); //
                                    
                                }
                                if (sum != 0)
                                {
                                    foreach (int j in indexesToSellInt) info.RemoveItem(illegalItemsInInventory[j - 1]);
                                    info.AddMoney(sum);
                                    exit = true;
                                    Console.Write($"Вы заработали {sum} брум. Нажмите любую клавишу для продолжения");
                                    Console.ReadKey();
                                }
                            }
                        }
                        if (exit) break;
                    }
                    else Console.WriteLine("Формат ввода был неверный, попробуйте еще раз!");
                }
            }
            public void SellJunk(ref Information info, ref int soldJunk)
            {
                int i = 1;
                bool exit = true;
                bool continueEarly = false;
                List<string> legalItems = info.possibleItems.Where(x => x.Value == false).Select(x => x.Key).ToList();
                List<string> legalItemsInInventory = new List<string>();
                foreach (string item in legalItems)
                {
                    if (info.inventory.Contains(item))
                    {
                        legalItemsInInventory = info.inventory.FindAll(x => x == item).Concat(legalItemsInInventory).ToList();
                        i++;
                    }
                }
                while (true)
                {
                    continueEarly = false;
                    Console.WriteLine("Вы можете продать следующие предметы из своего инвентаря: ");
                    if (i == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("В вашем инвентаре нет нужных предметов!");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        exit = true;
                    }
                    else
                    {
                        char[] separator = new char[] { ' ' };
                        int sum = 0;
                        for (int n = 0; n < legalItemsInInventory.Count; n++) Console.WriteLine($"\t{n + 1}) {legalItemsInInventory[n]}");
                        Console.Write("Введите номера предметов, которые хотите продать, в формате Цифра1 Цифра2 Цифра3. Если передумали, введите \"0\": ");
                        string str = Console.ReadLine();
                        if (str == "0") continue;
                        List<string> indexesToSell = str.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<int> indexesToSellInt = new List<int>();
                        foreach (string s in indexesToSell)
                        {
                            if (indexesToSell.FindAll(x => x == s).Count > 1)
                            {
                                Console.WriteLine("Вы ввели несколько одинаковых индексов, исправте ввод!");
                                continueEarly = true;
                                break;
                            }
                        }
                        if (continueEarly) continue;
                        foreach (string item in indexesToSell)
                        {
                            if (int.TryParse(item, out int value1) && value1 <= legalItemsInInventory.Count)
                            {
                                indexesToSellInt.Add(value1);
                                sum += 5;
                            }
                            else
                            {
                                Console.WriteLine("Какое-то из введеных чисел имело не тот формат, попробуйте еще раз!");
                                indexesToSellInt.Clear();
                                exit = false;
                                sum = 0;
                                break;
                            }
                            //foreach (int asd in indexesToSellInt) Console.WriteLine($"item number: {asd}"); //
                        }
                        if (sum != 0)
                        {
                            foreach (int j in indexesToSellInt) info.RemoveItem(legalItemsInInventory[j - 1]);
                            info.AddMoney(sum);
                            soldJunk += sum / 5;
                            exit = true;
                            Console.Write($"Вы заработали {sum} брум. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                        }
                    }
                    if (exit) break;
                }
            }
        }
        class Chill : Information
        {
            public string ChillType { get; private set; }
            public int StressRelief { get; private set; }
            public int MoneyRequired {  get; private set; }
            public List<string> ItemsRequired { get; private set; }
            public double DebuffGain { get; private set; }
            public Chill(string chillType, int stressRelief, int moneyRequired, List<string> itemsRequired, double debuffGain)
            {
                ChillType = chillType;
                StressRelief = stressRelief;
                MoneyRequired = moneyRequired;
                ItemsRequired = itemsRequired;
                DebuffGain = debuffGain;
            }
            public bool CheckChill(ref Information info) //дополнительно удаляет алко
            {
                List<string> itemsRequiredWithoutAlchohol = new List<string>();
                foreach (string item in ItemsRequired) if (item != "Пиво" && item != "Виски") itemsRequiredWithoutAlchohol.Add(item);
                if (info.Money < MoneyRequired)
                {
                    Console.Write("У Вас не хватает денег! Нажмите любую клавишу для продолжения");
                    Console.ReadKey();
                    return false;
                }
                if (ItemsRequired.Count != itemsRequiredWithoutAlchohol.Count)
                {
                    if (info.inventory.Contains("Пиво") || info.inventory.Contains("Виски"))
                    {
                        foreach (string item in itemsRequiredWithoutAlchohol)
                        {
                            if (info.inventory.Contains(item)) continue;
                            else
                            {
                                Console.WriteLine("У Вас отсутствуют необходимые предметы. Нажмите любую клавишу для продолжения");
                                Console.ReadKey();
                                return false;
                            }
                        }
                        /*if (info.inventory.Contains("Пиво"))
                        {
                            info.RemoveItem("Пиво");
                            return true;
                        }
                        if (info.inventory.Contains("Виски"))
                        {
                            info.RemoveItem("Виски");
                            return true;
                        }*/
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("У Вас отсутствуют необходимые предметы. Нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        return false;
                    }
                }
                else
                {
                    foreach (string item in ItemsRequired)
                    {
                        if (info.inventory.Contains(item)) continue;
                        else
                        {
                            Console.WriteLine("У Вас отсутствуют необходимые предметы. Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                            return false;
                        }
                    }
                    return true;
                }
            }
            public void DoChill(ref Information info)
            {
                List<string> itemsRequiredWithoutAlchohol = new List<string>();
                foreach (string item in ItemsRequired) if (item != "Пиво" && item != "Виски") itemsRequiredWithoutAlchohol.Add(item); 
                if (CheckChill(ref info))
                {
                    info.AddMoney(-MoneyRequired);
                    info.ChangeStress(-StressRelief);
                    info.AddDebuff(DebuffGain);
                    if (ItemsRequired.Count != itemsRequiredWithoutAlchohol.Count)
                    {
                        if (info.inventory.Contains("Пиво")) info.RemoveItem("Пиво");
                        else info.RemoveItem("Виски");
                    }
                    foreach (string item in itemsRequiredWithoutAlchohol)
                    {
                        info.RemoveItem(item);
                    }
                    if (ItemsRequired.Contains("Телевизор")) info.AddItem("Телевизор");
                    info.AddTime(2);
                    Console.Write("Отдых прошел успешно! Нажмите любую клавишу для продолжения");
                    Console.ReadKey();
                }
            }
            public string ShowRequiredItems()
            {
                //string str = String.Concat<string>(ItemsRequired.Select(x => x + " "));
                //if (MoneyRequired != 0) str += "Деньги: " + Convert.ToString(MoneyRequired) + " брум";
                //return str;
                return MoneyRequired == 0 ? String.Concat<string>(ItemsRequired.Select(x => x + " ")) : String.Concat<string>(ItemsRequired.Select(x => x + " ")) + "Деньги: " + Convert.ToString(MoneyRequired) + " брум";
            }
        }
        class Store : Information
        {
            public Dictionary<string, double> Stock = new Dictionary<string, double>();
            private Dictionary<string, double> storePossibleItems = new Dictionary<string, double>
            {
                { "Камера", 30 },
                { "Прослушка", 10 },
                { "Пиво", 10 },
                { "Виски", 15 },
                { "Пластинка(легал)", 15 },
                { "Видеокассета(легал)", 10 },
                { "Досье", 5 },
                { "Журнал", 15 },
                { "Цветы", 5 },
                { "Лекарства", 20 },
                { "Лекарства(имп)", 50 },
                { "Ваза", 5 },
                { "Сервиз", 5 },
                { "Сундучок", 5 },
                { "Провизия", 10 },
                { "Телевизор", 200 },
                { "Суп(5пров)", 45 },
            };
            private int _stockNumber = 5;
            public void ShowStock()
            {

            }
            public Dictionary<string, double> RefreshStock(Information info)
            {
                Stock.Clear();
                int foodPrice = 10;
                if (info.Week == 4)
                {
                    storePossibleItems.Remove("Пиво");
                    storePossibleItems.Remove("Виски");
                    storePossibleItems.Add("Пиво", 15);
                    storePossibleItems.Add("Виски", 20);
                }
                if (info.Week == 7) _stockNumber--;
                if (info.Week == 10)
                {
                    _stockNumber--;
                    storePossibleItems.Clear();
                    storePossibleItems = new Dictionary<string, double>
                    {
                        { "Камера", 35 },
                        { "Прослушка", 15 },
                        { "Пиво", 20 },
                        { "Виски", 25 },
                        { "Пластинка(легал)", 20 },
                        { "Видеокассета(легал)", 15 },
                        { "Досье", 5 },
                        { "Журнал", 20 },
                        { "Цветы", 5 },
                        { "Лекарства", 25 },
                        { "Лекарства(имп)", 55 },
                        { "Ваза", 5 },
                        { "Сервиз", 5 },
                        { "Сундучок", 5 },
                        { "Провизия", 15 },
                        { "Телевизор", 240 },
                        { "Суп(5пров)", 65 }
                    };
                    foodPrice += 5;
                }
                Random rand = new Random();
                List<int> indexes = new List<int>();
                int index = 0;
                for (int i = 0; i < _stockNumber; i++)
                {
                    index = rand.Next(0, storePossibleItems.Count);
                    if (!indexes.Contains(index)) indexes.Add(index);
                    else i--;
                    indexes.Sort();
                }
                foreach(int j in indexes) Stock.Add(storePossibleItems.ElementAt(j).Key, storePossibleItems.ElementAt(j).Value);
                if (info.Week != 12)
                {
                    Stock.Add("Провизия 1", foodPrice);
                    Stock.Add("Провизия 2", foodPrice);
                    Stock.Add("Провизия 3", foodPrice);
                    Stock.Add("Провизия 4", foodPrice);
                    Stock.Add("Провизия 5", foodPrice);
                }
                else Stock.Remove("Провизия"); Stock.Remove("Суп(5пров)");
                return Stock;
            }

        }
        class Occations : Information
        {
            public string Name { get; private set; }
            public int BotsMoney { get; private set; }
            public int BotsStress { get; private set; }
            public int Stress { get; private set; }
            public double Debuff { get; private set; }
            public string BotsMessage { get; private set; }
            public Occations(string name, int botsMoney, int botsStress, int stress, double debuff, string message)
            {
                Name = name;
                BotsMoney = botsMoney;
                BotsStress = botsStress;
                Stress = stress;
                Debuff = debuff;
                BotsMessage = message;
            }
            public Occations()
            {

            }
            public string GiveRandomOccasion()
            {
                Random rand = new Random();
                Dictionary<Events, int> eventsWithProbability = new Dictionary<Events, int>
                {

                    { Events.Драка, 0 },
                    { Events.Электричество, 21},
                    { Events.Морозы, 41},
                    { Events.Газ, 51},
                    { Events.Затопление, 61},
                    { Events.Задымление, 71},
                    { Events.Пожар, 81},
                    { Events.Забастовка, 89},
                    { Events.Обрушение, 95},
                    { Events.Подрыв, 98}

                };
                int i = rand.Next(1, 101);
                int j = 0;
                foreach (int value in eventsWithProbability.Values)
                {
                    //if (i >= 99) return eventsWithProbability.Last().Key;
                    //Console.WriteLine(i);
                    if (i - value <= 0) return eventsWithProbability.ElementAt(j - 1).Key.ToString();
                    j++;
                }
                return eventsWithProbability.Last().Key.ToString();
            }
            public Occations FindOccasion (List<Occations> occasions)
            {
                foreach (Occations occ in occasions)
                {
                    if (occ.Name == GiveRandomOccasion()) return occ;
                }
                return null;
            }
            public void ShowOccasion()
            {

            }
        } // не написан полностью, нужно переделывать многое в коде
    }
}
