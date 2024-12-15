using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mastermind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {

        //timer voor tijd bij te houden.
        private DispatcherTimer countdownTimer;
        private int countdownTime = 0;

        private int score = 100; // Startscore

        //lijst aanmaken voor de verschillende kleuren
        List<string> colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        List<string> chosenColors = new List<string>();
        //lijst voor het opslaan van de highscore
        private List<Tuple<string, int, int>> highScores = new List<Tuple<string, int, int>>();

        //aanmaken van variabele van attempts
        private int attempts = 1;
        private int maxAttempts = 10;
        string playerName;

        List<string> players = new List<string>();
        private int currentPlayerIndex = 0;

        private List<string> feedbackList = new List<string>();

        private Random random = new Random();

        // Variabele voor de kleurcode die we willen tonen in de titel
        private string colorCodeString = "";





        public MainWindow()
        {
            InitializeComponent();

           StartGame();

            //hier timer initialiseren
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);//elke seconden ticken
            countdownTimer.Tick += CountdownTimer_Tick;

            ComboBoxes();

            RandomColors();

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            tooltipLabel.Content = $"Witte rand: Juiste kleur, foute positien \nRode rand: Juiste kleur, juiste positie \nGeen kleur: Foute kleur ";

        }
        private void StartGame()
        {
            players.Clear();

            do
            {
                playerName = Interaction.InputBox($"Welkom, Geef uw naam.", "Welkom");

                if (!string.IsNullOrEmpty(playerName))
                {
                    players.Add(playerName); // add player in de List
                }
                else
                {
                    MessageBox.Show("Naam mag niet leeg zijn. Probeer opnieuw");
                }

                if(players.Count < 4)//Controle op minder dan 4 spelers
                {
                    var result = MessageBox.Show("Wil je nog een speler toevoegen?", "Nieuwe speler", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        break; // stoppen als de gebruiker nee zegt
                    }
                }
                
            } while (players.Count < 4); //blijven vragen tot er 4 spelers zijn

            Title = $"Mastermind - {players[currentPlayerIndex]} - Poging {attempts} van {maxAttempts}";
            MessageBox.Show($"Er zijn {players.Count} spelers toegevoegd.");
           
        }
        // Methode om de countdown timer te starten (of opnieuw te starten)
        /// <summary>
        /// Starten en herstarten van de countdowntimer
        /// Waarde van de timer tickt met 1 seconden verder omhoog
        /// </summary>
        private void startCountdown()
        {
            countdownTime = 1;  // Zet de tijd op 1 seconde bij elke start

            countdownTimer.Start(); // Start de timer
            timerLabel.Content = $"Tijd: {countdownTime} sec";
        }
        /// <summary>
        /// Stopt de countdowntimer en verhoogt het aantal pogingen 
        /// Timer stopt en wordt gereset naar 0, titel wordt bijgewerkt naar een nieuwe poging
        /// </summary>
        private void StopCountdown()
        {
            countdownTimer.Stop();

            // Verhoog het aantal pogingen (beurten)
            attempts++;
            if (attempts <= maxAttempts)
            {
                Title = $"Mastermind - Poging {attempts} van {maxAttempts}";
            }
            else
            {
                EndGame();
            }


            countdownTime = 0;
            timerLabel.Content = $"Tijd: {countdownTime} sec";
            startCountdown();  
        }

        // Event handler voor de countdown timer
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTime++;

            if (countdownTime >= 10)
            {
                StopCountdown();
            }

            timerLabel.Content = $"Tijd: {countdownTime} sec";
        }


        // Event handler voor de toetscombinatie CTRL+F12
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Controleer of CTRL+F12 is ingedrukt
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F12)
            {
                ToggleDebug(); 
            }
        }



        /// <summary>
        /// Hier wordt de verborgen label weergegeven die de kleurcode voorziet van het spel
        /// Als de debugTextBox zichtbaar is, wordt deze verborgen.
        /// </summary>
        private void ToggleDebug()
        {
            // Als de debugTextBox zichtbaar is, verberg het en anders toon je de textbox
            if (debugTextBox.Visibility == Visibility.Collapsed)
            {
                debugTextBox.Visibility = Visibility.Visible;
                debugTextBox.Text = colorCodeString;
            }
            else
            {
                debugTextBox.Visibility = Visibility.Collapsed;
            }
        }
        private void RandomColors()
        {
            
            // 4 kleuren leegmaken anders kan dit problemen veroorzaken
            chosenColors.Clear();

            // genereer random kleuren, 4 stuks
            Random random = new Random();
            for (int i = 0; i < 4; i++) 
            { 
             int index = random.Next(colors.Count);
                chosenColors.Add(colors[index]);
            
            }
            string colorstring = string.Join(",", chosenColors);

            //titel updaten met de code
            colorCodeString = string.Join(",", chosenColors);

            

            startCountdown();
        }

        private void ComboBoxes()
        {
            comboBox1.ItemsSource = colors;
            comboBox2.ItemsSource = colors;
            comboBox3.ItemsSource = colors;
            comboBox4.ItemsSource = colors;

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedItem != null)
                label1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBox1.SelectedItem.ToString()));
            if (comboBox2.SelectedItem != null)
                label2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBox2.SelectedItem.ToString()));
            if (comboBox3.SelectedItem != null)
                label3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBox3.SelectedItem.ToString()));
            if (comboBox4.SelectedItem != null)
                label4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBox4.SelectedItem.ToString()));

        }
        private void UpdateFeedbackDisplay()
        {
            // Clear the ListBox before adding new items
            attemptsListBox.Items.Clear();

            // Voeg elke poging en feedback toe aan de ListBox
            foreach (var feedback in feedbackList)
            {
                // Maak een StackPanel voor elke poging
                StackPanel attemptPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                // Voeg de vier kleuren toe als Borders met achtergrondkleur
                for (int i = 0; i < 4; i++)
                {
                    Border colorBorder = new Border
                    {
                        Width = 30,
                        Height = 30,
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(chosenColors[i]))
                    };
                    attemptPanel.Children.Add(colorBorder);
                }

                // Voeg de feedback (rood/wit) toe aan de StackPanel
                TextBlock feedbackTextBlock = new TextBlock
                {
                    Text = feedback,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0),
                    FontSize = 16
                };
                attemptPanel.Children.Add(feedbackTextBlock);

                // Voeg de StackPanel toe aan de ListBox
                attemptsListBox.Items.Add(attemptPanel);
            }
        }

        private int attemptCount = 0; // Aantal pogingen
        private bool gameWon = false; // Of de speler de code heeft gekraakt

        private void validateButton_Click(object sender, RoutedEventArgs e)
        {
            Title = $"Mastermind - {players[currentPlayerIndex]} - Poging {attempts} van {maxAttempts}";

            // Verkrijg de geselecteerde kleuren van de comboboxen
            string selectedColor1 = comboBox1.SelectedItem?.ToString();
            string selectedColor2 = comboBox2.SelectedItem?.ToString();
            string selectedColor3 = comboBox3.SelectedItem?.ToString();
            string selectedColor4 = comboBox4.SelectedItem?.ToString();

            // Controleer of alle comboboxen geselecteerd zijn
            if (selectedColor1 == null || selectedColor2 == null || selectedColor3 == null || selectedColor4 == null)
            {
                MessageBox.Show("Kies alstublieft een kleur voor elke combobox.");
                return;
            }

            // Zet de borders van de labels terug naar de standaard (geen rand)
            label1.BorderBrush = null;
            label2.BorderBrush = null;
            label3.BorderBrush = null;
            label4.BorderBrush = null;

            // We maken een tijdelijke lijst van de gekozen kleuren (voor feedback)
            List<string> selectedColors = new List<string> { selectedColor1, selectedColor2, selectedColor3, selectedColor4 };
            List<Border> colorBorders = new List<Border>();  // List om de borders van de ingevoerde kleuren op te slaan

            // Feedback variabelen
            int redCount = 0;
            int whiteCount = 0;
            List<string> tempChosenColors = new List<string>(chosenColors); // Kopie van de geheime code

            // We gaan voor elke geselecteerde kleur controleren of deze correct is (rood/wit)
            for (int i = 0; i < 4; i++)
            {
                // Maak een Border voor elke geselecteerde kleur
                Border colorBorder = new Border
                {
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedColors[i])),
                    BorderThickness = new Thickness(2)  // Zorg ervoor dat de rand zichtbaar is
                };
                colorBorders.Add(colorBorder);

                // Feedback bepalen
                if (selectedColors[i] == chosenColors[i]) // Kleur en positie zijn correct
                {
                    colorBorder.BorderBrush = new SolidColorBrush(Colors.DarkRed);  // Rood voor correct
                    redCount++;
                    tempChosenColors[i] = ""; // Verwijder de correct geraden kleur uit de lijst

                    colorBorder.ToolTip = "Juiste kleur, juiste positie";
                }
                else if (chosenColors.Contains(selectedColors[i])) // Kleur is aanwezig, maar op de verkeerde plaats
                {
                    colorBorder.BorderBrush = new SolidColorBrush(Colors.Wheat); // Wit voor incorrecte positie
                    whiteCount++;

                   
                    int tempIndex = tempChosenColors.IndexOf(selectedColors[i]);
                    if (tempIndex != -1) // Zorg ervoor dat de kleur gevonden wordt in tempChosenColors
                    {
                        tempChosenColors[tempIndex] = ""; // Verwijder de kleur uit de lijst
                    }
                    colorBorder.ToolTip = "Juiste kleur, foute positie";
                }

                else
                {
                    // Kleur die helemaal niet voorkomt krijgt geen border (optioneel)
                    colorBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    colorBorder.ToolTip = "Foute kleur";
                }
            }

            // Voeg de kleuren en de feedback toe aan de lijst
            string feedback = $"{redCount} rood, {whiteCount} wit";

            // Voeg een StackPanel toe voor deze poging met de vier gekozen kleuren en de feedback
            StackPanel attemptPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // Voeg de kleuren (Borders) toe aan het StackPanel
            foreach (var border in colorBorders)
            {
                attemptPanel.Children.Add(border);
            }

            // Voeg de feedback als tekst toe
            TextBlock feedbackTextBlock = new TextBlock
            {
                Text = feedback,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                FontSize = 16
            };
            attemptPanel.Children.Add(feedbackTextBlock);

            // Voeg de poging toe aan de ListBox
            attemptsListBox.Items.Add(attemptPanel);

            // Stop de countdown en verhoog de poging
            StopCountdown();

            // **Score berekening**
            
            int incorrectColors = 4 - redCount - whiteCount;
            score -= (whiteCount * 1) + (incorrectColors * 2);
            scoreLabel.Content = $"Score: {score}";

            // **Check of het spel afgelopen is**
            attemptCount++; // Verhoog de poging teller

            // Check of de speler de code heeft gekraakt (4 rood)
            if (redCount == 4)
            {
                gameWon = true;
                MessageBox.Show($"Gefeliciteerd! Je hebt de code gekraakt in {attemptCount} pogingen!");
                EndGame();
                return;
            }



            // Als we het aantal pogingen hebben bereikt
            if (attemptCount >= maxAttempts)
            {
                // Show a message when the player reaches the max attempts
          
                EndGame();
            }
            Title = $"Mastermind - {players[currentPlayerIndex]} - Poging {attempts} van {maxAttempts}";
        }
        private void GenerateNewCode()
        {
            
            chosenColors.Clear();  

            for (int i = 0; i < 4; i++)
            {
                int index = random.Next(colors.Count);  
                chosenColors.Add(colors[index]);       
            }

           
        }



        
        private void EndGame()
        {
            if (players.Count > 0)
            {
               
                if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
                {  
                    highScores.Add(new Tuple<string, int, int>(players[currentPlayerIndex], score, attemptCount));
                }
                else
                {
                    MessageBox.Show("Fout bij het toevoegen van score. Ongeldige speler index.");
                }
            }
            else
            {
                MessageBox.Show("Er zijn geen spelers om scores aan toe te voegen.");
            }

            //Order highscore op score, score hetzelfde dan order op attempts
            highScores = highScores.OrderByDescending(x => x.Item2).ThenBy(x => x.Item3).ToList();

            
            if (highScores.Count > 15)
            {
                highScores = highScores.Take(15).ToList();
            }

            var result = MessageBox.Show("Wil je opnieuw spelen?", "Spel beëindigen", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                
                GenerateNewCode();
                ResetGame();
            }
        }


        

       
        private void ResetGame()
        {
            attemptCount = 0;
            attempts = 1;
            gameWon = false;
            score = 100;
            scoreLabel.Content = $"Score: {score}";  
            attemptsListBox.Items.Clear();     
                                             
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            comboBox3.SelectedItem = null;
            comboBox4.SelectedItem = null;

            currentPlayerIndex = 0;
        }
        //private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
           // if (!gameWon && attemptCount < maxAttempts)  
           // {
              //  var result = MessageBox.Show("Het spel is nog niet beëindigd. Weet je zeker dat je de applicatie wilt afsluiten?",
                                           //  "Beëindigen",
                                             //MessageBoxButton.YesNo,
                                             //MessageBoxImage.Warning);
                //if (result == MessageBoxResult.Yes)
                //{
                   // Application.Current.Shutdown();  
                //}
                //else
                //{
                   // e.Cancel = true; 
                //}
            //}
        //}

        private void MnuNewGame_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewCode();
            ResetGame();
            //Update de debug label met de nieuwe code
            colorCodeString = string.Join(",", chosenColors);
        }

        private void MnuHighscore_Click(object sender, RoutedEventArgs e)
        {

            string highScoreMessage = "Top 15 High Scores \n\n";

            for(int i = 0; i < highScores.Count; i++)
            {
                string playerName = highScores[i].Item1;  //naam
                int playerScore = highScores[i].Item2;    //score
                int playerAttempts = highScores[i].Item3; //attempts    

                highScoreMessage += $"{i+1}.{playerName}- {playerScore} punten, {playerAttempts} pogingen\n";
            }
            MessageBox.Show(highScoreMessage,"Mastermind Highscores", MessageBoxButton.OK);
        }

        private void MnuClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MnuAttempts_Click(object sender, RoutedEventArgs e)
        {
            string answer = Interaction.InputBox("Geef een getal", "Aantal pogingen", "10", 20);

            int inputAttampts;
            bool isValid = int.TryParse(answer, out inputAttampts);

            if (isValid && inputAttampts >= 3 && inputAttampts <= 20)
            {
                maxAttempts = inputAttampts;
                MessageBox.Show($"Aantal pogingen ingesteld op {maxAttempts}.", "Instelling succesvol");

                attempts = 1;
                Title = $"Mastermind - Poging {attempts} van {maxAttempts}";

            }
            else
            {
                MessageBox.Show("Ongeldige invoer. Kies een getal tussen 3 en 20.", "Foutieve invoer");
            }

            Title = $"Mastermind - Poging 1 van {maxAttempts}";


        }

        private void hintButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (score >= 15)
            {
                
                string hintChoice = Interaction.InputBox("Kies je hint: \n\n1. Juiste kleur \n2. Juiste kleur op juiste plaats",
                                                        "Kies een hint",
                                                        "1",  
                                                        0, 0);

                if (hintChoice == "1")
                {
                    if (score >= 15) //check voor genoeg punten
                    {
                        score -= 15;
                        MessageBox.Show("De geheime kleur bevat een van de volgende kleuren: " + GetCorrectColorHint(), "Juiste Kleur Hint", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Je hebt niet genoeg punten om deze hint te kopen.", "Fout");
                    }
                }
                else if (hintChoice == "2")
                {
                    if (score >= 25) //check voor genoeg punten
                    {
                        score -= 25;
                        MessageBox.Show("Een van de kleuren in de juiste volgorde is: " + GetCorrectColorPositionHint(), "Juiste Kleur op Juiste Plaats Hint", MessageBoxButton.OK);

                    }
                    else
                    {
                        MessageBox.Show("Je hebt niet genoeg punten om deze hint te kopen.", "Fout");
                    }
                }
                else
                {
                    MessageBox.Show("Ongeldige keuze. Probeer opnieuw.", "Fout");
                }

                
                scoreLabel.Content = $"Score: {score}"; //update score
            }
            else
            {
                MessageBox.Show("Niet genoeg punten. 15 punten of meer nodig.");
            }
           
        }
        
        private string GetCorrectColorHint()
        {
            
            List<string> possibleColors = new List<string>();
            foreach (var color in chosenColors)
            {
                if (!possibleColors.Contains(color)) //geen duplicates
                {
                    possibleColors.Add(color);
                }
            }

            
            Random random = new Random();
            return possibleColors[random.Next(possibleColors.Count)];
        }

       
        private string GetCorrectColorPositionHint()
        {
            
            List<int> correctPositionIndexes = new List<int>();
            for (int i = 0; i < chosenColors.Count; i++)
            {
                //controleer dat de kleur in de juiste positie staat
                if (comboBox1.SelectedItem?.ToString() == chosenColors[i])
                {
                    correctPositionIndexes.Add(i);
                }
            }

            
            Random random = new Random();
            int randomIndex = correctPositionIndexes[random.Next(correctPositionIndexes.Count)];

            return $"Positie {randomIndex + 1}: {chosenColors[randomIndex]}"; 
        }
    }
}


