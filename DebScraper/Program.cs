using DebScraper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DebScraper;

public class Program
{
    static string _gameUrl = "https://deb-online.live/spielbericht/?gameId=";

    static IWebDriver _driver = new ChromeDriver();

    static string _gameIdFile = "C:\\Users\\public\\Documents\\DEB\\GameIds.txt";

    static List<Game> _myGames = new List<Game>();

    static int _yearOfTaxes = 2022;

    static List<liga> _ligen { get; set; } = new List<liga>();

    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine(" --- Start Webscraping");
            _yearOfTaxes = getYear();
            _ligen = GetLigen();
            if _ligen.Count = 0
                

            foreach (var liga in _ligen)
            {
                _driver.Navigate().GoToUrl(liga.sourceUrl);

                System.Threading.Thread.Sleep(500);

                IWebElement CookieBoxSaveButton = _driver.FindElement(By.Id("CookieBoxSaveButton"));
                CookieBoxSaveButton.Click();

                System.Threading.Thread.Sleep(500);
                Liga manuellLiga = new Liga()
                                   {
                                       name = liga.name,
                                       sourceUrl = liga.sourceUrl,
                                       Ids = Ligen.ReadFileLines(_gameIdFile);
                                   };
                                   }
                List<string> GameIds = Ligen.ReadFileLines(_gameIdFile);

                foreach (var GameId in GameIds)
                {
                    try
                    {
                        string GameNumber = GameIds.FirstOrDefault(game => game == GameId);
                        string SpecificGameUrl = _gameUrl + GameId;
                        _driver.Navigate().GoToUrl(SpecificGameUrl);
                        System.Threading.Thread.Sleep(1000);
                        IWebElement buttonSpielbericht = _driver.FindElement(By.Id("btn-report"));
                        buttonSpielbericht.Click();
                        System.Threading.Thread.Sleep(1000);

                        //var element = driver.FindElement(By.ClassName("#pbp_report > div > div.-hd-los-game-full-report-game-facts > div.-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-linesman1 > div.-hd-los-game-full-report-game-fact-value"));
                        List<IWebElement> RefGespann = new List<IWebElement>();
                        RefGespann.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-linesman1 .-hd-los-game-full-report-game-fact-value > div")));
                        RefGespann.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-linesman2 .-hd-los-game-full-report-game-fact-value > div")));
                        RefGespann.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-referee1 .-hd-los-game-full-report-game-fact-value > div")));
                        RefGespann.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-referee2 .-hd-los-game-full-report-game-fact-value > div")));

                        System.Threading.Thread.Sleep(500);

                        if (RefGespann.FirstOrDefault(refs => refs.Text.ToLower().Contains("filips")) != null)
                        {
                            Console.WriteLine(" --- Game found: " + SpecificGameUrl);
                            Game game = new Game()
                                        {
                                            url = SpecificGameUrl,
                                            Guid = Guid.Parse(GameId),
                                            liga = "",
                                        };

                            IWebElement dateElement = _driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row-scheduledDate .-hd-los-game-full-report-game-fact-value > div"));
                            DateTime.TryParse(dateElement.Text, out DateTime date);
                            game.date = date;
                            game.ort = _driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row-location .-hd-los-game-full-report-game-fact-value > div")).Text;

                            if (date.Year == _yearOfTaxes)
                            {
                                _myGames.Add(game);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" --- Exception: " + e.Message);
                        Console.WriteLine(" --- Exception: " + GameId);
                    }
                }

                Console.WriteLine(" --- Finished Webscraping");

                Console.WriteLine($" ------ Full List of Games found in {_yearOfTaxes} - there were {_myGames.Count.ToString()} Games foundt ------");
                _myGames.Sort((x, y) => DateTime.Compare(x.date, y.date));
                foreach (var game in _myGames)
                {
                    Console.WriteLine(" --- Game found in 2022: " + game.date.Date + ";" + game.ort + ";" + game.liga + ";" + _gameUrl + game.Guid);
                }

                Console.WriteLine("------ Tax Analyze done. Thank you! ------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(" --- Big exception" + ex.Message);
        }

        {
            // Treiber schließen
            _driver.Quit();
        }
    }

    public static List<liga> GetLigen(int year)
    {
        List<liga> ligen = new List<liga>();
        string oberligaSued = "https://deb-online.live/liga/herren/oberliga-sued/";
        ligen.Add(new liga
                  {
                      name = "Oberliga Süd",
                      sourceUrl = oberligaSued,
                      Ids = GetIds(oberligaSued, year)
                  });
        
        string oberligaNord = "https://deb-online.live/liga/herren/oberliga-nord/";
        ligen.Add(new liga
                  {
                        name = "Oberliga Nord",
                      sourceUrl = oberligaNord,
                      Ids = GetIds(oberligaNord, year)
                  });
        string frauen = "https://deb-online.live/liga/damen/bundesliga/";
        ligen.Add(new liga
                  {
                      name = "Frauen",
                      sourceUrl = frauen,
                      Ids = GetIds(frauen, year)
                  });
        string frauenPokal = "https://deb-online.live/liga/damen/deb-pokal-frauen/";
        ligen.Add(new liga
                  {
                      name = "Frauen Pokal",
                      sourceUrl = frauenPokal,
                      Ids = GetIds(frauenPokal, year)
                  });

        string dnl = "https://deb-online.live/liga/herren/u20-dnl/";
        ligen.Add(new liga
                  {
                      name = "DNL",
                      sourceUrl = dnl,
                      Ids = GetIds(dnl, year)
                  });
        return ligen;
    }

    public static List<Guid> GetIds(string sourceUrlLiga, int season)
    {
        return new List<Guid>();
    }

    public static int getYear()
    {
        try
        {
            Console.WriteLine(" --- Please enter the year of the taxes: ");
            string input = Console.ReadLine();
            return int.Parse(input);
        }
        catch (Exception e)
        {
            return 0;
        }
    }
}

// //        var actions = new Actions(driver);
// var dropDownDivs = driver.FindElement(By.CssSelector("div.-hd-util-select-display.-hd-clickable"));
// actions.MoveToElement(dropDownDivs).Click().Perform();
//         
// IWebElement dropDownDiv = driver.FindElement(By.CssSelector("div.-hd-util-select-display.-hd-clickable"));
// dropDownDiv.Click();
//         
// var selectElement = new SelectElement(driver.FindElement(By.CssSelector(".-hd-los-division-picker-select-season")));
// selectElement.SelectByText("Saison 2022/23");
//         
// IWebElement dropDownSaisons = driver.FindElement(By.CssSelector("-hd-util-select"));
// dropDownSaisons.Click();
//         
//         
// IWebElement dropDownSaison = driver.FindElement(By.CssSelector("div.-hd-util-select-display.-hd-clickable"));
// dropDownSaison.Click();

// public void GetAllIdsForNextYear()
// {
//     IWebElement listItemErgebnisse = driver.FindElement(By.Id("ergebnisse"));
//     listItemErgebnisse.Click();
//
//     System.Threading.Thread.Sleep(3000);
//
//     IWebElement elemt = driver.FindElement(By.CssSelector("-hd-util-select "));
//
//     //elemt. = "-hd-util-select -hd-util-select-open"; 
//
//     WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
//     IWebElement selectElementVisible = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".-hd-los-division-picker-select-season")));
//
//     IWebElement table = driver.FindElement(By.TagName("table"));
//
//     //manuell Saison auswählen
//     ReadOnlyCollection<IWebElement> rows = table.FindElements(By.TagName("tr"));
//
//     foreach (IWebElement row in rows)
//     {
//         ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
//         foreach (IWebElement cell in cells)
//         {
//             Console.WriteLine(cell.Text);
//         }
//     }
// }
// }
