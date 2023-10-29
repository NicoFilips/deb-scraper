using DebScraper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

// Notiz: Länderspiele vom DEB in Email Rechnung nachschauen

IWebDriver _driver = new ChromeDriver();
string _gameIdFile = "C:\\Users\\nicof\\Desktop\\del\\IDs\\GameIds.txt";
List<Game> MyGames = new List<Game>();

{
    Console.WriteLine(" --- Start Webscraping");

    List<liga> ligen = new List<liga>();
    ligen.Add(new liga
              {
                  sourceUrl = "https://deb-online.live/liga/herren/oberliga-sued/",
                  foundGamesUrl = new List<string>()
              });

    foreach (var liga in ligen)
    {
        _driver.Navigate().GoToUrl(liga.sourceUrl);

        System.Threading.Thread.Sleep(500);

        IWebElement CookieBoxSaveButton = _driver.FindElement(By.Id("CookieBoxSaveButton"));
        CookieBoxSaveButton.Click();

        System.Threading.Thread.Sleep(500);
        List<string> GameIds = Ligen.ReadFileLines(_gameIdFile);
        
        foreach (var GameId in GameIds)
        {
            try
            {
                string gameUrl = "https://deb-online.live/spielbericht/?gameId=" + GameId;
                _driver.Navigate().GoToUrl(gameUrl);
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
                    Console.WriteLine(" --- Game found: " + gameUrl);
                    Game game = new Game()
                                {
                                    date = DateTime.Now,
                                    liga = liga.sourceUrl,
                                    homeTeam = "homeTeam",
                                    Guid = Guid.NewGuid()
                                };
                        
                    IWebElement dateElement = _driver.FindElement(By.CssSelector(".-hd-los-game-full-report-game-fact-row-scheduledDate .-hd-los-game-full-report-game-fact-value > div"));
                    DateTime.TryParse(dateElement.Text, out DateTime date);
                    if (date.Year == 2022)
                    {
                        MyGames.Add(gameUrl);
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
MyGames.
        foreach (var game in MyGames)
        {
            Console.WriteLine(" --- Game found in 2022: " + game);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(" --- Big exception" + ex.Message);
}
finally
{
    // Treiber schließen
    _driver.Quit();
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
