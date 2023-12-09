// Ходы делаются по очереди.
// В каждый ход называется число, тоже четырехзначное и с разными цифрами.
// Если цифра из названного числа есть в отгадываемом числе,
//      то эта ситуация называется «корова».
// Если цифра из названного числа есть в отгадываемом числе и стоит в том же месте,
//      то эта ситуация называется «бык». 
// Например, первый игрок задумал 8569, а второй игрок назвал 8974
//      То первый игрок должен сказать: «Один бык и одна корова» (1б,1к)


//-------------------------------------------------------------

int DIGITS_NUMBERS = 4; // кол-во цифр в числе
double NUMBERS_MIN = Math.Pow(10, (DIGITS_NUMBERS-1));  // минимальное число
double NUMBERS_MAX = Math.Pow(10, DIGITS_NUMBERS)-1;    // минимальное число

// Функция "корова/бык" - проверяет:
//  1) есть ли цифры из первого числа во втором числе (корова)
//  2) eсть ли цифры из первого числа в отгадываемом числе и стоят ли в том же месте (бык),
// number - проверяемое число
// numberHidden - загаданное число
Tuple<int, int> CheckDigitСowBull(int number, int numberHidden)
{
    int cow = 0;    // коровы
    int bull = 0;   // быки
    // сравниваем все цифры числа первого числа со вторым
    // слева направо в двух вложенных циклах
    // сравниваю цифры в текстовом представвлении
    string numberString = number.ToString();
    string numberHiddenString = numberHidden.ToString();
    for (int i = 0; i < DIGITS_NUMBERS; i++)
    {
        char num1 = numberString[i];
        for (int j = 0; j < DIGITS_NUMBERS; j++)
        {
            char num2 = numberHiddenString[j];
            if (num1 == num2)   // если цифры совпали
            {
                cow += 1;
                if (i == j)     // если цифры совпали и стоят на тех-же местах
                {
                    bull += 1;
                }
            }
        }
    }
    return Tuple.Create(cow, bull);
}

// Функция проверки числа "number" на различие всех цифр
// true - если цифры различные, false - если нет
bool CheckDiffDigitInNumber(int number)
{    
    // сравниваем все цифры числа друг с другом
    // слева направо в двух вложенных циклах
    string numberString = number.ToString();
    for (int i = 0; i < DIGITS_NUMBERS-1; i++)
    {
        char num1 = numberString[i];
        for (int j = i + 1; j < DIGITS_NUMBERS; j++)
        {
            char num2 = numberString[j];
            if (num1 == num2) return false;
        }
    }
    return true;
}

// Интерфейс ввода с терминала с проверкой на валидность,
// возвращает число удовлетворяющее правилам игры (0 - выход)
int InterfaceGame ()
{
    int number;
    while (true)
    {
        {
            System.Console.Write("Введите положительное четырехзначное число с разными цифрами (0 - выход): ");
            string? number_text = Console.ReadLine();
            if (number_text == "0")
                return 0; // 0 - завершение игры

            // проверка введенного значения на число
            bool success = int.TryParse(number_text, out number);

            // проверка на корректность введенных данных
            if (success)    // если введеное значение - это число, то проверяем остальные условия
            {
                if (number < 0) // если число отрицательное
                {
                    System.Console.WriteLine(" >> Некорректный запрос! Введено отрицательное число!");
                }
                else if (number < NUMBERS_MIN | number > NUMBERS_MAX)   // если число не входит в диапазон
                {
                    System.Console.WriteLine(" >> Некорректный запрос! Введено не четырхзначное число!");
                }
                else if (!CheckDiffDigitInNumber(number))  // если введенное число содержит одинаковые цифры
                {
                    System.Console.WriteLine(" >> Некорректный запрос! Введено число c одинаковыми цифрами!");
                }
                else // возвращаем введенное число, т.е. все норм!
                {
                    return number;
                }
            }
            else // введенное значение - не число, т.к. содержит другие симоволы кроме цифр
                Console.WriteLine(" >> Некорректный запрос! присутствуют симоволы отличные от цифр");
        }
    }
}

// печать архивной записи в терминале
void PrintRecord (Dictionary<int, Tuple<int, string>> record)
{
    System.Console.WriteLine();
    foreach(var pair in record)
    {
        var (number, result) = pair.Value;
        Console.WriteLine($"{pair.Key}: {number} -> {result}");
    }
    System.Console.WriteLine();
}


//---------------- БЛОК ПРОГРАММЫ -------------------
System.Console.WriteLine("Игра <<Быки и коровы>>");

// 1. Загадываем число, удовлетворяющее требованию (четырехзначное, положительное, разные цифры)
int number_hidden;
while (true)
{
    number_hidden = new Random().Next((int)NUMBERS_MIN, (int)(NUMBERS_MAX)+1);
    if (CheckDiffDigitInNumber (number_hidden)) break;
}
System.Console.WriteLine("Загадано число от 1000 до 9999. Попробуй угадать его!");
// Console.WriteLine($" >>> загаданное число = {number_hidden}");


// 2. Инициализируем словарь для записи истории
var record_game  = new Dictionary<int, Tuple<int, string>>(); // архив-запись вводимых чисел и результатов, под индексом 0 - загаданное число
int count = 1; // счетчик попыток

// 3. Запуск бесконечного цикла ввода с терминала
while (true)
{
    PrintRecord (record_game);
    System.Console.WriteLine($"Попытка: {count}");
    int number_try = InterfaceGame (); // получаем число из интерфейса
    if (number_hidden == number_try)   // угадали?
    {
        System.Console.WriteLine($"Число угадано! (за {count} попыток)\n");
        break;
    }
    if (number_try == 0)               // 0 - завершение игры
    {
        System.Console.WriteLine(" -- остановка игры --");
        break;
    }
    var (cow, bull) = CheckDigitСowBull (number_try, number_hidden); // проверяем на "коровы" и "быки"
    string str_cow_bull = $"{bull} бык., {cow} коров.";              // формируем строковое представление результата
    System.Console.WriteLine(str_cow_bull);
    record_game.Add (count, Tuple.Create(number_try, str_cow_bull)); // записываем результат в архив
    count += 1;
}