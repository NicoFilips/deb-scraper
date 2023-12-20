using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DebScraper;

using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

public class Program
{
    private static string _gameUrl = "https://deb-online.live/spielbericht/?gameId=";

    private static IWebDriver _driver = new ChromeDriver();

    private static string _gameIdFile = "C:\\Users\\public\\Documents\\DEB\\GameIds.txt";

    private static List<Game> _myGames = new List<Game>();

    private static int _yearOfTaxes = 2022;

    private static string _refName { get; set; } = string.Empty;

    private static List<liga> _ligen { get; set; } = new List<liga>();

    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine(" --- Start Webscraping");

#if DEBUG
            _yearOfTaxes = DateTime.Now.Year - 1;
#else
            _yearOfTaxes = getYear();
            if (_yearOfTaxes == 0)
            {
                _yearOfTaxes = DateTime.Now.Year - 1;
            }
#endif
#if DEBUG
            _refName = "filips";
#else
            _refName = getName();
            if (_refName == "")
            {
                _refName = "filips";
            }
#endif

            _ligen = GetLigen(_yearOfTaxes);
            if (_ligen == null)
            {
                _ligen = new List<liga>();
                liga manuellLiga = new liga()
                                   {
                                       name = "Textfile GameIds",
                                       Ids = Ligen.ReadFileLines(_gameIdFile)
                                   };
                _ligen.Add(manuellLiga);
            }

            _driver.Navigate().GoToUrl("https://deb-online.live/");
            System.Threading.Thread.Sleep(500);

            IWebElement CookieBoxSaveButton = _driver.FindElement(By.Id("CookieBoxSaveButton"));
            CookieBoxSaveButton.Click();

            System.Threading.Thread.Sleep(500);

            foreach (var debLiga in _ligen)
            {
                foreach (var GameId in debLiga.Ids)
                {
                    try
                    {
                        //string GameNumber = GameId.ToString().FirstOrDefault(game => game == GameId);
                        string SpecificGameUrl = _gameUrl + GameId;
                        _driver.Navigate().GoToUrl(SpecificGameUrl);
                        System.Threading.Thread.Sleep(1000);
                        IWebElement buttonSpielbericht = _driver.FindElement(By.Id("btn-report"));
                        buttonSpielbericht.Click();
                        System.Threading.Thread.Sleep(1000);

                        //var element = driver.FindElement(By.ClassName("#pbp_report > div > div.-hd-los-game-full-report-game-facts > div.-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-linesman1 > div.-hd-los-game-full-report-game-fact-value"));
                        List<IWebElement> GameInfo = new List<IWebElement>();
                        GameInfo.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-linesman1 .-hd-los-game-full-report-game-fact-value > div")));
                        GameInfo.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-linesman2 .-hd-los-game-full-report-game-fact-value > div")));
                        GameInfo.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-referee1 .-hd-los-game-full-report-game-fact-value > div")));
                        GameInfo.Add(_driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row.-hd-los-game-full-report-game-fact-row-referee2 .-hd-los-game-full-report-game-fact-value > div")));
                        System.Threading.Thread.Sleep(500);

                        if (GameInfo.FirstOrDefault(refs => refs.Text.ToLower().Contains(_refName)) != null)
                        {
                            Console.WriteLine(" --- Game found: " + SpecificGameUrl);
                            Game game = new Game()
                                        {
                                            liga = debLiga.name,
                                            url = SpecificGameUrl,
                                            Guid = GameId,
                                        };

                            IWebElement HomeTeam = _driver.FindElement(By.ClassName("sv_links_title"));
                            IWebElement timeElement = _driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row-scheduledTime .-hd-los-game-full-report-game-fact-value > div"));
                            IWebElement dateElement = _driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row-scheduledDate .-hd-los-game-full-report-game-fact-value > div"));
                            DateTime.TryParse(dateElement.Text + " " + timeElement.Text, out DateTime date);
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
            }

            Console.WriteLine(" --- Finished Webscraping");

            Console.WriteLine($" ------ Full List of Games found in {_yearOfTaxes} - there were {_myGames.Count.ToString()} Games found ------");
            _myGames.Sort((x, y) => DateTime.Compare(x.date, y.date));
            foreach (var game in _myGames)
            {
                Console.WriteLine(" --- Game found in 2022: " + game.date.Date + ";" + game.ort + ";" + game.liga + ";" + _gameUrl + game.Guid);
            }

            Console.WriteLine("------ Tax Analyze done. Thank you! ------");
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
        return null;
        // ----- Oberliga -----
        List<liga> ligen = new List<liga>();
        string oberligaSued = "https://deb-online.live/liga/herren/oberliga-sued/";
        ligen.Add(new liga
                  {
                      name = "Oberliga Süd",
                      sourceUrl = oberligaSued,
                      Ids = GetIds(oberligaSued, year)
                  });

        string oberligaTest = "https://deb-online.live/liga/herren/oberliga-testspiele/";
        ligen.Add(new liga
                  {
                      name = "Oberliga Testgames",
                      sourceUrl = oberligaTest,
                      Ids = GetIds(oberligaTest, year)
                  });

        string oberligaNord = "https://deb-online.live/liga/herren/oberliga-nord/";
        ligen.Add(new liga
                  {
                      name = "Oberliga Nord",
                      sourceUrl = oberligaNord,
                      Ids = GetIds(oberligaNord, year)
                  });

        // ----- Frauen -----

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

        string frauenTest = "https://deb-online.live/liga/damen/bundesliga/";
        ligen.Add(new liga
                  {
                      name = "Frauen Testgames",
                      sourceUrl = frauenTest,
                      Ids = GetIds(frauenTest, year)
                  });

        // ----- DNL -----

        string dnl = "https://deb-online.live/liga/herren/u20-dnl/";
        ligen.Add(new liga
                  {
                      name = "DNL",
                      sourceUrl = dnl,
                      Ids = GetIds(dnl, year)
                  });

        string dnl20Test = "https://deb-online.live/liga/herren/u20-dnl-testspiele/";
        ligen.Add(new liga
                  {
                      name = "DNL Testgames",
                      sourceUrl = dnl20Test,
                      Ids = GetIds(dnl20Test, year)
                  });
        return ligen;
    }

    public static List<Guid> GetIds(string sourceUrlLiga, int season)
    {
        //Todo: implement logic to get all Ids from the sourceUrlLiga
        _driver.Navigate().GoToUrl(sourceUrlLiga);
        System.Threading.Thread.Sleep(500);

        IWebElement CookieBoxSaveButton = _driver.FindElement(By.Id("CookieBoxSaveButton"));
        CookieBoxSaveButton.Click();

        System.Threading.Thread.Sleep(500);
        IWebElement listItemErgebnisse = _driver.FindElement(By.Id("ergebnisse"));
        listItemErgebnisse.Click();
        
        System.Threading.Thread.Sleep(2000);
        
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5000));
        wait.Until((d) =>
                   {
                       d.ExecuteJavaScript("return jQuery.active == 0");
                       bool pageUpdated = d.FindElement(By.TagName("table")).Displayed; // oder ein anderes Indiz für das Laden
                       return true && pageUpdated;
                   });
        IWebElement table = _driver.FindElement(By.TagName("table"));
        
        IList<IWebElement> rows = table.FindElements(By.CssSelector("tr.classname"));
        
        IWebElement dropDownDiv = _driver.FindElement(By.CssSelector("div.-hd-util-select-display.-hd-clickable"));
        dropDownDiv.Click();
        
        return new List<Guid>();
    }

    public static int getYear()
    {
        try
        {
            Console.WriteLine(" --- Please enter the season for scraping the data: ");
            string input = Console.ReadLine();
            return int.Parse(input);
        }
        catch (Exception e)
        {
            return 0;
        }
    }

    public static string getName()
    {
        try
        {
            Console.WriteLine(" --- Please enter your name for scraping the data: ");
            Console.WriteLine(" --- Note: Its enough to get your last or first name if its unique, else enter your lastname before your firstname");
            string input = Console.ReadLine().TrimEnd().TrimStart().ToLowerInvariant();
            return input;
        }
        catch (Exception e)
        {
            return "";
        }
    }
}

// var actions = new Actions(driver);
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
