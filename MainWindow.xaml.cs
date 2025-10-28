using System;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace CatholicWordle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            // Initialize WebView2
            await webView.EnsureCoreWebView2Async(null);
            
            // Get the HTML content
            string htmlContent = GetGameHtml();
            
            // Load the HTML
            webView.NavigateToString(htmlContent);
        }

        private string GetGameHtml()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Catholic Wordle</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 10px;
        }

        .container {
            background: white;
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            max-width: 600px;
            width: 100%;
        }

        h1 {
            color: #667eea;
            text-align: center;
            margin-bottom: 15px;
            font-size: 1.8em;
        }

        h2 {
            color: #333;
            text-align: center;
            margin-bottom: 10px;
            font-size: 1.2em;
        }

        .page {
            display: none;
        }

        .page.active {
            display: block;
        }

        .game-option {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px;
            margin: 10px 0;
            border-radius: 10px;
            cursor: pointer;
            transition: transform 0.2s, box-shadow 0.2s;
            text-align: center;
            font-size: 1.1em;
            font-weight: bold;
        }

        .game-option:hover {
            transform: translateY(-3px);
            box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
        }

        .info {
            text-align: center;
            color: #666;
            margin: 8px 0;
            font-size: 0.9em;
        }

        .game-board {
            display: flex;
            flex-direction: column;
            gap: 5px;
            margin: 15px 0;
            justify-content: center;
            align-items: center;
        }

        .guess-row {
            display: flex;
            gap: 5px;
            justify-content: center;
        }

        .letter-box {
            width: 50px;
            height: 50px;
            border: 2px solid #d3d6da;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.8em;
            font-weight: bold;
            text-transform: uppercase;
            border-radius: 4px;
            transition: all 0.3s;
            background-color: white;
        }

        .letter-box.filled {
            border-color: #878a8c;
        }

        .letter-box.correct {
            background-color: #6aaa64;
            border-color: #6aaa64;
            color: white;
        }

        .letter-box.present {
            background-color: #c9b458;
            border-color: #c9b458;
            color: white;
        }

        .letter-box.absent {
            background-color: #787c7e;
            border-color: #787c7e;
            color: white;
        }

        .letter-box.active-box {
            border-color: #667eea;
            border-width: 3px;
            animation: pulse 0.5s ease-in-out;
        }

        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.05); }
        }

        .keyboard {
            margin-top: 15px;
        }

        .keyboard-row {
            display: flex;
            justify-content: center;
            gap: 4px;
            margin-bottom: 6px;
        }

        .key {
            background-color: #d3d6da;
            color: #000;
            border: none;
            border-radius: 4px;
            padding: 12px;
            font-size: 0.9em;
            font-weight: bold;
            cursor: pointer;
            min-width: 30px;
            transition: all 0.1s;
            user-select: none;
        }

        .key:hover {
            filter: brightness(0.9);
        }

        .key:active {
            transform: scale(0.95);
        }

        .key.wide {
            min-width: 55px;
            font-size: 0.75em;
        }

        .key.correct {
            background-color: #6aaa64;
            color: white;
        }

        .key.present {
            background-color: #c9b458;
            color: white;
        }

        .key.absent {
            background-color: #787c7e;
            color: white;
        }

        button.action-btn {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 10px 20px;
            font-size: 0.95em;
            border-radius: 8px;
            cursor: pointer;
            margin: 8px 4px;
            transition: transform 0.2s;
            font-weight: bold;
        }

        button.action-btn:hover {
            transform: scale(1.05);
        }

        .message {
            text-align: center;
            font-size: 1.1em;
            margin: 15px 0;
            padding: 15px;
            border-radius: 10px;
            font-weight: bold;
        }

        .message.success {
            background-color: #d4edda;
            color: #155724;
        }

        .message.failure {
            background-color: #f8d7da;
            color: #721c24;
        }

        .answer-reveal {
            text-align: center;
            font-size: 1.3em;
            margin: 15px 0;
            padding: 12px;
            background-color: #f0f0f0;
            border-radius: 8px;
        }

        .answer-reveal span {
            color: #667eea;
            font-weight: bold;
        }

        .button-group {
            text-align: center;
            margin-top: 15px;
        }

        .error-shake {
            animation: shake 0.5s;
        }

        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-10px); }
            75% { transform: translateX(10px); }
        }

        .word-input-container {
            margin: 15px 0;
            text-align: center;
            position: relative;
        }

        .word-input {
            font-size: 1.2em;
            padding: 10px 40px 10px 10px;
            border: 2px solid #667eea;
            border-radius: 8px;
            text-align: center;
            width: 200px;
            text-transform: uppercase;
            margin-bottom: 10px;
        }

        .word-input.password {
            -webkit-text-security: disc;
            text-security: disc;
        }

        .word-input:focus {
            outline: none;
            border-color: #764ba2;
            box-shadow: 0 0 5px rgba(102, 126, 234, 0.5);
        }

        .toggle-visibility {
            position: absolute;
            right: 10px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            cursor: pointer;
            padding: 5px;
            border-radius: 4px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .toggle-visibility:hover {
            background-color: #f0f0f0;
        }

        .toggle-visibility svg {
            width: 20px;
            height: 20px;
            fill: #666;
        }

        .toggle-visibility:hover svg {
            fill: #333;
        }

        .number-input {
            font-size: 1.2em;
            padding: 10px;
            border: 2px solid #667eea;
            border-radius: 8px;
            text-align: center;
            width: 120px;
            margin: 5px;
        }

        .number-input:focus {
            outline: none;
            border-color: #764ba2;
            box-shadow: 0 0 5px rgba(102, 126, 234, 0.5);
        }

        .suggestions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            justify-content: center;
            margin: 15px 0;
        }

        .suggestion {
            background: #f0f0f0;
            padding: 8px 12px;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.2s;
            font-weight: bold;
        }

        .suggestion:hover {
            background: #667eea;
            color: white;
            transform: scale(1.05);
        }

        .input-info {
            color: #666;
            font-size: 0.9em;
            margin: 5px 0;
        }

        .input-group {
            margin: 15px 0;
            text-align: center;
        }

        .input-label {
            display: block;
            margin-bottom: 5px;
            color: #333;
            font-weight: bold;
        }

        .config-options {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            justify-content: center;
            margin: 20px 0;
        }

        .config-option {
            text-align: center;
        }

        .loading {
            text-align: center;
            padding: 10px;
            color: #666;
        }

        .spinner {
            border: 3px solid #f3f3f3;
            border-top: 3px solid #667eea;
            border-radius: 50%;
            width: 30px;
            height: 30px;
            animation: spin 1s linear infinite;
            margin: 0 auto 10px;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        .api-status {
            text-align: center;
            margin: 10px 0;
            padding: 8px;
            border-radius: 5px;
            font-size: 0.9em;
        }

        .api-status.online {
            background-color: #d4edda;
            color: #155724;
        }

        .api-status.offline {
            background-color: #f8d7da;
            color: #721c24;
        }

        .generated-word-container {
            margin: 15px 0;
            text-align: center;
            position: relative;
        }

        .generated-word-display {
            font-size: 1.2em;
            padding: 10px 40px 10px 10px;
            border: 2px solid #28a745;
            border-radius: 8px;
            text-align: center;
            width: 200px;
            background-color: #f8f9fa;
            margin-bottom: 10px;
            -webkit-text-security: disc;
            text-security: disc;
        }

        .generated-word-display.revealed {
            -webkit-text-security: none;
            text-security: none;
            background-color: #e9f7ef;
        }

        .word-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
            margin: 15px 0;
        }

        .word-action-btn {
            background: #6c757d;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.2s;
        }

        .word-action-btn:hover {
            background: #5a6268;
        }

        .word-action-btn.primary {
            background: #28a745;
        }

        .word-action-btn.primary:hover {
            background: #218838;
        }

        .word-action-btn.warning {
            background: #ffc107;
            color: #212529;
        }

        .word-action-btn.warning:hover {
            background: #e0a800;
        }

        .catholic-actions, .original-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
            margin: 15px 0;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div id=""menuPage"" class=""page active"">
            <h1>🎮 Wordle Games</h1>
            <div class=""game-option"" onclick=""selectGame('catholic')"">
                ✝️ Catholic Wordle
            </div>
            <div class=""game-option"" onclick=""selectGame('original')"">
                🎯 Original Wordle
            </div>
            <div class=""game-option"" onclick=""selectGame('custom')"">
                ⚙️ Custom Wordle
            </div>
            <div class=""game-option"" onclick=""selectGame('random')"">
                🎲 Random Wordle
            </div>
        </div>

        <div id=""setupPage"" class=""page"">
            <h1 id=""setupTitle"">Setup Game</h1>
            
            <div id=""apiStatus"" class=""api-status"" style=""display: none;""></div>
            
            <div id=""catholicConfig"" class=""config-options"" style=""display: none;"">
                <div class=""word-input-container"">
                    <input type=""text"" id=""catholicTargetWord"" class=""word-input password"" maxlength=""5"" 
                           placeholder=""Enter 5-letter word"" oninput=""validateWordInput(this)"">
                    <button class=""toggle-visibility"" onclick=""togglePasswordVisibility('catholicTargetWord')"" title=""Show/hide word"">
                        <svg viewBox=""0 0 24 24"" width=""20"" height=""20"">
                            <path d=""M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z""/>
                        </svg>
                    </button>
                    <div class=""input-info"">Enter a 5-letter Catholic word or generate a random one</div>
                </div>
                
                <div class=""catholic-actions"">
                    <button class=""word-action-btn primary"" onclick=""generateCatholicWord()"">Generate Random Catholic Word</button>
                </div>
                
                <h2>Or choose a suggestion:</h2>
                <div class=""suggestions"" id=""catholicSuggestions""></div>
            </div>

            <div id=""originalConfig"" class=""config-options"" style=""display: none;"">
                <div class=""word-input-container"">
                    <input type=""text"" id=""originalTargetWord"" class=""word-input password"" maxlength=""5"" 
                           placeholder=""Enter 5-letter word"" oninput=""validateWordInput(this)"">
                    <button class=""toggle-visibility"" onclick=""togglePasswordVisibility('originalTargetWord')"" title=""Show/hide word"">
                        <svg viewBox=""0 0 24 24"" width=""20"" height=""20"">
                            <path d=""M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z""/>
                        </svg>
                    </button>
                    <div class=""input-info"">Enter a 5-letter word or generate a random general word</div>
                </div>
                
                <div class=""original-actions"">
                    <button class=""word-action-btn primary"" onclick=""generateOriginalWord()"">Generate Random General Word</button>
                </div>
                
                <h2>Or choose a suggestion:</h2>
                <div class=""suggestions"" id=""originalSuggestions""></div>
            </div>

            <div id=""customConfig"" class=""config-options"" style=""display: none;"">
                <div class=""input-group"">
                    <label class=""input-label"">Word Length</label>
                    <input type=""number"" id=""customWordLength"" class=""number-input"" 
                           min=""3"" max=""15"" value=""5"" onchange=""updateCustomConfig()"">
                </div>
                
                <div class=""input-group"">
                    <label class=""input-label"">Max Attempts</label>
                    <input type=""number"" id=""customMaxAttempts"" class=""number-input"" 
                           min=""1"" max=""20"" value=""6"" onchange=""updateCustomConfig()"">
                </div>
                
                <div class=""word-input-container"">
                    <input type=""text"" id=""customTargetWord"" class=""word-input password"" 
                           placeholder=""Enter your word"" oninput=""validateCustomWordInput(this)"">
                    <button class=""toggle-visibility"" onclick=""togglePasswordVisibility('customTargetWord')"" title=""Show/hide word"">
                        <svg viewBox=""0 0 24 24"" width=""20"" height=""20"">
                            <path d=""M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z""/>
                        </svg>
                    </button>
                    <div class=""input-info"">Enter a custom word (length will be adjusted)</div>
                </div>
            </div>

            <div id=""randomConfig"" class=""config-options"" style=""display: none;"">
                <div class=""input-group"">
                    <label class=""input-label"">Word Length</label>
                    <input type=""number"" id=""randomWordLength"" class=""number-input"" 
                           min=""3"" max=""15"" value=""5"">
                </div>
                
                <div class=""input-group"">
                    <label class=""input-label"">Max Attempts</label>
                    <input type=""number"" id=""randomMaxAttempts"" class=""number-input"" 
                           min=""1"" max=""20"" value=""6"">
                </div>
                
                <div class=""input-info"">A random word will be generated based on your settings</div>
                <div class=""input-info"">Uses online API when available for more variety</div>
                
                <div class=""word-actions"">
                    <button class=""word-action-btn primary"" onclick=""generateRandomWord()"">Generate Random Word</button>
                </div>
                
                <div id=""generatedWordSection"" style=""display: none;"">
                    <div class=""generated-word-container"">
                        <input type=""text"" id=""generatedWordDisplay"" class=""generated-word-display"" readonly>
                        <button class=""toggle-visibility"" onclick=""toggleGeneratedWordVisibility()"" title=""Show/hide word"">
                            <svg viewBox=""0 0 24 24"" width=""20"" height=""20"">
                                <path d=""M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z""/>
                            </svg>
                        </button>
                        <div class=""input-info"">Generated word (click eye to reveal)</div>
                    </div>
                    
                    <div class=""word-actions"">
                        <button class=""word-action-btn warning"" onclick=""generateRandomWord()"">Generate New Word</button>
                        <button class=""word-action-btn primary"" onclick=""useGeneratedWord()"">Use This Word</button>
                    </div>
                </div>
                
                <div id=""randomLoading"" class=""loading"" style=""display: none;"">
                    <div class=""spinner""></div>
                    <div>Generating word...</div>
                </div>
            </div>
            
            <div class=""button-group"">
                <button class=""action-btn"" onclick=""startGameWithConfig()"" id=""startButton"">Start Game</button>
                <button class=""action-btn"" onclick=""goToMenu()"">Back to Menu</button>
            </div>
        </div>

        <div id=""gamePage"" class=""page"">
            <h1 id=""gameTitle"">✝️ Catholic Wordle</h1>
            <div class=""info"">
                <p>Attempts: <span id=""attemptsLeft"">6</span> / <span id=""maxAttemptsDisplay"">6</span></p>
                <p>Word Length: <span id=""wordLengthDisplay"">5</span> letters</p>
            </div>
            
            <div class=""game-board"" id=""gameBoard""></div>
            
            <div class=""keyboard"" id=""keyboard"">
                <!-- Keyboard will be generated dynamically -->
            </div>

            <div class=""button-group"">
                <button class=""action-btn"" onclick=""goToMenu()"">Main Menu</button>
            </div>
        </div>

        <div id=""resultPage"" class=""page"">
            <h1 id=""resultTitle"">Game Over!</h1>
            <div id=""resultMessage"" class=""message""></div>
            <div id=""answerReveal"" class=""answer-reveal""></div>
            
            <div class=""button-group"">
                <button class=""action-btn"" onclick=""viewPuzzle()"">View Puzzle</button>
                <button class=""action-btn"" onclick=""playAgain()"">Play Again</button>
                <button class=""action-btn"" onclick=""goToMenu()"">Main Menu</button>
            </div>
        </div>

        <div id=""puzzlePage"" class=""page"">
            <h1>Your Puzzle</h1>
            <div class=""game-board"" id=""puzzleBoard""></div>
            <div class=""answer-reveal"" id=""puzzleAnswer""></div>
            
            <div class=""button-group"">
                <button class=""action-btn"" onclick=""backToResult()"">Back</button>
                <button class=""action-btn"" onclick=""playAgain()"">Play Again</button>
                <button class=""action-btn"" onclick=""goToMenu()"">Main Menu</button>
            </div>
        </div>
    </div>

    <script>
        let currentGame = '';
        let targetWord = '';
        let currentAttempt = 0;
        let currentGuess = '';
        let maxAttempts = 6;
        let wordLength = 5;
        let guesses = [];
        let gameActive = false;
        let keyboardState = {};
        let apiAvailable = false;
        let generatedRandomWord = '';

        // Enhanced Catholic word library (5-letter words only for Catholic mode)
        const catholicWords = [
            'FAITH', 'GRACE', 'SAINT', 'CROSS', 'ALTAR', 'ANGEL', 'BLESS', 'JESUS', 
            'LIGHT', 'MERCY', 'PEACE', 'PRAYER', 'GLORY', 'HOLY', 'DIVINE', 'SACRED',
            'ROSARY', 'MASS', 'HOST', 'BIBLE', 'CHURCH', 'PRIEST', 'MONK', 'NUN',
            'SAVIOR', 'LORD', 'GOD', 'SON', 'SPIRIT', 'TRUTH', 'VIRTUE', 'SOUL',
            'HEAVEN', 'SIN', 'DEATH', 'LIFE', 'LOVE', 'HOPE', 'JOY', 'DEAR'
        ];

        // Enhanced general word library for Original Wordle (5-letter words)
        const generalWords = [
            'APPLE', 'BEACH', 'CHAIR', 'DANCE', 'EARTH', 'FLAME', 'GRAPE', 'HOUSE',
            'IGLOO', 'JUICE', 'KNIFE', 'LEMON', 'MUSIC', 'NIGHT', 'OCEAN', 'PIANO',
            'QUEEN', 'RIVER', 'SMILE', 'TIGER', 'UMBRA', 'VOICE', 'WATER', 'XENON',
            'YACHT', 'ZEBRA', 'BRAVE', 'CLOUD', 'DREAM', 'EAGLE', 'FLOWER', 'GARDEN',
            'HAPPY', 'ISLAND', 'JUNGLE', 'KITTEN', 'LIGHT', 'MOUNTAIN', 'NATURE',
            'OCEAN', 'PENCIL', 'QUIET', 'RABBIT', 'SUNSET', 'TRAVEL', 'UNIVERSE',
            'VICTORY', 'WONDER', 'YOUTH', 'ZEPHYR', 'BUTTERFLY', 'COMPUTER',
            'DIAMOND', 'ELEPHANT', 'FRIENDSHIP', 'GALAXY', 'HARMONY', 'ILLUSION',
            'JOURNEY', 'KEYBOARD', 'LANDSCAPE', 'MYSTERY', 'NOTEBOOK', 'OPPORTUNITY',
            'BANANA', 'CASTLE', 'DONKEY', 'EAGER', 'FROST', 'GLOBE', 'HONEY', 'IVORY',
            'JELLY', 'KOALA', 'LILAC', 'MANGO', 'NINJA', 'OLIVE', 'PUPPY', 'QUILT',
            'ROBIN', 'SNAIL', 'TOWEL', 'ULTRA', 'VIOLA', 'WHALE', 'XEROX', 'YOGURT', 'ZESTY'
        ];

        // Enhanced word library for other modes
        const wordLibrary = {
            3: ['GOD', 'SON', 'JOY', 'LAW', 'ART', 'SKY', 'SEA', 'ICE', 'FOG', 'MAP'],
            4: ['PRAY', 'HOLY', 'FAITH', 'HOPE', 'LOVE', 'SOUL', 'BLESS', 'PEACE', 'GRACE', 'TRUTH'],
            5: ['FAITH', 'GRACE', 'SAINT', 'CROSS', 'ALTAR', 'ANGEL', 'BLESS', 'JESUS', 'LIGHT', 'MERCY'],
            6: ['PRAYER', 'CHURCH', 'GOSPEL', 'DIVINE', 'SACRED', 'BIBLE', 'PRIEST', 'SERMON', 'SAVIOR', 'HEAVEN'],
            7: ['BAPTISM', 'COMMUNE', 'HOLINESS', 'MIRACLE', 'PASSOVER', 'SALVATION', 'TRINITY', 'WORSHIP'],
            8: ['BENEDICT', 'CATHOLIC', 'EUCHARIST', 'RESURRECT', 'SACRAMENT', 'SCRIPTURE'],
            9: ['APOSTOLIC', 'BLESSEDNESS', 'CONFESSION', 'REDEMPTION', 'SANCTUARY'],
            10: ['BENEDICTION', 'CONFESSIONAL', 'EVANGELICAL', 'RESURRECTION'],
            11: ['COMMUNION', 'CONSECRATION', 'TESTAMENT'],
            12: ['CONFESSIONALS', 'EVANGELIZATION'],
            13: ['SACRAMENTAL'],
            14: [],
            15: []
        };

        // Complete the word library
        for (let len = 3; len <= 15; len++) {
            if (!wordLibrary[len] || wordLibrary[len].length === 0) {
                wordLibrary[len] = generalWords.filter(word => word.length === len);
            }
        }

        // Test API availability (only for Random Wordle)
        async function testAPI() {
            try {
                const response = await fetch('https://random-word-api.herokuapp.com/word?length=5');
                if (response.ok) {
                    const words = await response.json();
                    apiAvailable = words && words.length > 0;
                }
            } catch (error) {
                apiAvailable = false;
            }
            updateAPIStatus();
        }

        function updateAPIStatus() {
            const statusElement = document.getElementById('apiStatus');
            if (currentGame === 'random') {
                if (apiAvailable) {
                    statusElement.textContent = '✓ Online API Available - More word variety';
                    statusElement.className = 'api-status online';
                } else {
                    statusElement.textContent = '✗ Using Local Word List - Still plenty of words!';
                    statusElement.className = 'api-status offline';
                }
                statusElement.style.display = 'block';
            } else {
                statusElement.style.display = 'none';
            }
        }

        // Random Word API - Only used for Random Wordle
        async function getRandomWordFromAPI(length, count = 1) {
            if (!apiAvailable) {
                throw new Error('API not available');
            }

            try {
                const response = await fetch(`https://random-word-api.herokuapp.com/word?length=${length}&number=${count}`);
                if (!response.ok) {
                    throw new Error('API response not ok');
                }
                const words = await response.json();
                return words.map(word => word.toUpperCase());
            } catch (error) {
                console.error('API Error:', error);
                throw new Error('Failed to fetch word from API');
            }
        }

        function selectGame(gameType) {
            currentGame = gameType;
            if (gameType === 'random') {
                testAPI(); // Only test API for Random Wordle
            }
            showSetupPage(gameType);
        }

        function showSetupPage(gameType) {
            const title = document.getElementById('setupTitle');
            const catholicConfig = document.getElementById('catholicConfig');
            const originalConfig = document.getElementById('originalConfig');
            const customConfig = document.getElementById('customConfig');
            const randomConfig = document.getElementById('randomConfig');
            
            // Hide all configs first
            catholicConfig.style.display = 'none';
            originalConfig.style.display = 'none';
            customConfig.style.display = 'none';
            randomConfig.style.display = 'none';
            
            switch (gameType) {
                case 'catholic':
                    title.textContent = '✝️ Catholic Wordle Setup';
                    catholicConfig.style.display = 'block';
                    generateCatholicSuggestions();
                    break;
                case 'original':
                    title.textContent = '🎯 Original Wordle Setup';
                    originalConfig.style.display = 'block';
                    generateOriginalSuggestions();
                    break;
                case 'custom':
                    title.textContent = '⚙️ Custom Wordle Setup';
                    customConfig.style.display = 'flex';
                    updateCustomConfig();
                    break;
                case 'random':
                    title.textContent = '🎲 Random Wordle Setup';
                    randomConfig.style.display = 'flex';
                    // Reset generated word section
                    document.getElementById('generatedWordSection').style.display = 'none';
                    generatedRandomWord = '';
                    break;
            }
            
            showPage('setupPage');
        }

        // Generate a random Catholic word
        function generateCatholicWord() {
            const randomIndex = Math.floor(Math.random() * catholicWords.length);
            const randomWord = catholicWords[randomIndex];
            
            // Set the word in the input field
            const input = document.getElementById('catholicTargetWord');
            input.value = randomWord;
            
            // Ensure it's censored
            input.classList.add('password');
        }

        // Generate a random general word for Original Wordle
        function generateOriginalWord() {
            const randomIndex = Math.floor(Math.random() * generalWords.length);
            const randomWord = generalWords[randomIndex];
            
            // Set the word in the input field
            const input = document.getElementById('originalTargetWord');
            input.value = randomWord;
            
            // Ensure it's censored
            input.classList.add('password');
        }

        // Generate suggestions for Catholic Wordle
        function generateCatholicSuggestions() {
            const suggestionsContainer = document.getElementById('catholicSuggestions');
            suggestionsContainer.innerHTML = '';

            // Shuffle and take 6 random Catholic words
            const shuffled = [...catholicWords].sort(() => 0.5 - Math.random());
            const suggestions = shuffled.slice(0, 6);

            suggestions.forEach(word => {
                const suggestion = document.createElement('div');
                suggestion.className = 'suggestion';
                suggestion.textContent = word;
                suggestion.onclick = () => {
                    document.getElementById('catholicTargetWord').value = word;
                };
                suggestionsContainer.appendChild(suggestion);
            });

            document.getElementById('catholicTargetWord').value = '';
        }

        // Generate suggestions for Original Wordle (using general words)
        function generateOriginalSuggestions() {
            const suggestionsContainer = document.getElementById('originalSuggestions');
            suggestionsContainer.innerHTML = '';

            // Shuffle and take 6 random general words
            const shuffled = [...generalWords].sort(() => 0.5 - Math.random());
            const suggestions = shuffled.slice(0, 6);

            suggestions.forEach(word => {
                const suggestion = document.createElement('div');
                suggestion.className = 'suggestion';
                suggestion.textContent = word;
                suggestion.onclick = () => {
                    document.getElementById('originalTargetWord').value = word;
                };
                suggestionsContainer.appendChild(suggestion);
            });

            document.getElementById('originalTargetWord').value = '';
        }

        // Toggle password visibility for input fields
        function togglePasswordVisibility(inputId) {
            const input = document.getElementById(inputId);
            if (input.classList.contains('password')) {
                input.classList.remove('password');
            } else {
                input.classList.add('password');
            }
        }

        // Toggle generated word visibility
        function toggleGeneratedWordVisibility() {
            const display = document.getElementById('generatedWordDisplay');
            if (display.classList.contains('revealed')) {
                display.classList.remove('revealed');
            } else {
                display.classList.add('revealed');
            }
        }

        function updateCustomConfig() {
            const wordLength = parseInt(document.getElementById('customWordLength').value) || 5;
            const targetInput = document.getElementById('customTargetWord');
            targetInput.maxLength = wordLength;
            targetInput.placeholder = `Enter ${wordLength}-letter word`;
            
            if (targetInput.value.length > wordLength) {
                targetInput.value = targetInput.value.substring(0, wordLength);
            }
        }

        // Generate a random word for Random Wordle
        async function generateRandomWord() {
            const length = parseInt(document.getElementById('randomWordLength').value) || 5;
            
            // Show loading state
            document.getElementById('randomLoading').style.display = 'block';
            document.getElementById('generatedWordSection').style.display = 'none';
            
            let word = '';
            
            try {
                // Try API first
                if (apiAvailable) {
                    const apiWords = await getRandomWordFromAPI(length, 1);
                    word = apiWords[0];
                }
            } catch (error) {
                console.log('Using local word list');
            }
            
            // Fallback to local words
            if (!word) {
                const localWords = wordLibrary[length] || generalWords.filter(w => w.length === length);
                if (localWords.length === 0) {
                    alert('No words available for this length. Please choose a different length.');
                    document.getElementById('randomLoading').style.display = 'none';
                    return;
                }
                word = localWords[Math.floor(Math.random() * localWords.length)];
            }
            
            generatedRandomWord = word;
            
            // Update the display
            const display = document.getElementById('generatedWordDisplay');
            display.value = word;
            display.classList.remove('revealed'); // Reset to censored
            
            // Hide loading and show generated word section
            document.getElementById('randomLoading').style.display = 'none';
            document.getElementById('generatedWordSection').style.display = 'block';
        }

        // Use the generated word for Random Wordle
        function useGeneratedWord() {
            if (!generatedRandomWord) {
                alert('Please generate a word first.');
                return;
            }
            
            const length = parseInt(document.getElementById('randomWordLength').value) || 5;
            const attempts = parseInt(document.getElementById('randomMaxAttempts').value) || 6;
            
            targetWord = generatedRandomWord;
            maxAttempts = attempts;
            wordLength = length;
            
            startGame();
        }

        function validateWordInput(input) {
            input.value = input.value.toUpperCase().replace(/[^A-Z]/g, '');
            if (input.value.length > 5) {
                input.value = input.value.slice(0, 5);
            }
        }

        function validateCustomWordInput(input) {
            const wordLength = parseInt(document.getElementById('customWordLength').value) || 5;
            input.value = input.value.toUpperCase().replace(/[^A-Z]/g, '');
            if (input.value.length > wordLength) {
                input.value = input.value.slice(0, wordLength);
            }
        }

        function startGameWithConfig() {
            let word, attempts, length;

            switch (currentGame) {
                case 'catholic':
                    word = document.getElementById('catholicTargetWord').value.trim().toUpperCase();
                    if (word.length !== 5) {
                        alert('Please enter a 5-letter word, generate a random Catholic word, or select a suggestion.');
                        return;
                    }
                    if (!/^[A-Z]{5}$/.test(word)) {
                        alert('Please enter only letters A-Z.');
                        return;
                    }
                    targetWord = word;
                    maxAttempts = 6;
                    wordLength = 5;
                    break;

                case 'original':
                    word = document.getElementById('originalTargetWord').value.trim().toUpperCase();
                    if (word.length !== 5) {
                        alert('Please enter a 5-letter word, generate a random general word, or select a suggestion.');
                        return;
                    }
                    if (!/^[A-Z]{5}$/.test(word)) {
                        alert('Please enter only letters A-Z.');
                        return;
                    }
                    targetWord = word;
                    maxAttempts = 6;
                    wordLength = 5;
                    break;

                case 'custom':
                    word = document.getElementById('customTargetWord').value.trim().toUpperCase();
                    length = parseInt(document.getElementById('customWordLength').value) || 5;
                    attempts = parseInt(document.getElementById('customMaxAttempts').value) || 6;
                    
                    if (word.length !== length) {
                        alert(`Please enter a ${length}-letter word.`);
                        return;
                    }
                    if (!/^[A-Z]+$/.test(word)) {
                        alert('Please enter only letters A-Z.');
                        return;
                    }
                    targetWord = word;
                    maxAttempts = attempts;
                    wordLength = length;
                    break;

                case 'random':
                    if (!generatedRandomWord) {
                        alert('Please generate a random word first.');
                        return;
                    }
                    length = parseInt(document.getElementById('randomWordLength').value) || 5;
                    attempts = parseInt(document.getElementById('randomMaxAttempts').value) || 6;
                    targetWord = generatedRandomWord;
                    maxAttempts = attempts;
                    wordLength = length;
                    break;
            }

            startGame();
        }

        function startGame() {
            currentAttempt = 0;
            currentGuess = '';
            guesses = [];
            gameActive = true;
            keyboardState = {};
            
            // Set game title
            const gameTitle = document.getElementById('gameTitle');
            switch (currentGame) {
                case 'catholic': gameTitle.textContent = '✝️ Catholic Wordle'; break;
                case 'original': gameTitle.textContent = '🎯 Original Wordle'; break;
                case 'custom': gameTitle.textContent = '⚙️ Custom Wordle'; break;
                case 'random': gameTitle.textContent = '🎲 Random Wordle'; break;
            }
            
            // Update displays
            document.getElementById('maxAttemptsDisplay').textContent = maxAttempts;
            document.getElementById('wordLengthDisplay').textContent = wordLength;
            
            initializeBoard();
            initializeKeyboard();
            showPage('gamePage');
            updateAttemptsDisplay();
        }

        // ... (keep all the existing game functions like initializeBoard, handleKeyPress, etc.) ...
        function initializeBoard() {
            const board = document.getElementById('gameBoard');
            board.innerHTML = '';
            
            const boxSize = wordLength > 8 ? 40 : 50;
            const boxStyle = `width: ${boxSize}px; height: ${boxSize}px; font-size: ${wordLength > 8 ? '1.4em' : '1.8em'}`;
            
            for (let i = 0; i < maxAttempts; i++) {
                const row = document.createElement('div');
                row.className = 'guess-row';
                row.id = 'row-' + i;
                
                for (let j = 0; j < wordLength; j++) {
                    const box = document.createElement('div');
                    box.className = 'letter-box';
                    box.id = 'box-' + i + '-' + j;
                    box.style.cssText = boxStyle;
                    row.appendChild(box);
                }
                
                board.appendChild(row);
            }
        }

        function initializeKeyboard() {
            const keyboard = document.getElementById('keyboard');
            keyboard.innerHTML = '';
            
            const keyboardRows = [
                'QWERTYUIOP'.split(''),
                'ASDFGHJKL'.split(''),
                ['ENTER', ...'ZXCVBNM'.split(''), 'BACKSPACE']
            ];
            
            keyboardRows.forEach((rowKeys, rowIndex) => {
                const row = document.createElement('div');
                row.className = 'keyboard-row';
                
                rowKeys.forEach(key => {
                    const keyElement = document.createElement('div');
                    keyElement.className = 'key';
                    keyElement.textContent = key;
                    keyElement.setAttribute('data-key', key);
                    
                    if (key === 'ENTER' || key === 'BACKSPACE') {
                        keyElement.classList.add('wide');
                        keyElement.style.fontSize = '0.75em';
                    }
                    
                    keyElement.addEventListener('click', () => handleKeyPress(key));
                    row.appendChild(keyElement);
                });
                
                keyboard.appendChild(row);
            });
        }

        function handleKeyPress(key) {
            if (!gameActive) return;

            if (key === 'ENTER') {
                submitGuess();
            } else if (key === 'BACKSPACE') {
                if (currentGuess.length > 0) {
                    currentGuess = currentGuess.slice(0, -1);
                    updateCurrentRow();
                }
            } else if (currentGuess.length < wordLength && /^[A-Z]$/.test(key)) {
                currentGuess += key;
                updateCurrentRow();
            }
        }

        function updateCurrentRow() {
            for (let i = 0; i < wordLength; i++) {
                const box = document.getElementById('box-' + currentAttempt + '-' + i);
                if (i < currentGuess.length) {
                    box.textContent = currentGuess[i];
                    box.classList.add('filled');
                    if (i === currentGuess.length - 1) {
                        box.classList.add('active-box');
                    } else {
                        box.classList.remove('active-box');
                    }
                } else {
                    box.textContent = '';
                    box.classList.remove('filled', 'active-box');
                }
            }
        }

        function submitGuess() {
            if (currentGuess.length !== wordLength) {
                shakeRow(currentAttempt);
                return;
            }
            
            processGuess(currentGuess);
            guesses.push(currentGuess);
            
            if (currentGuess === targetWord) {
                setTimeout(() => endGame(true), 800);
            } else if (currentAttempt >= maxAttempts - 1) {
                setTimeout(() => endGame(false), 800);
            } else {
                currentAttempt++;
                currentGuess = '';
                updateAttemptsDisplay();
            }
        }

        function shakeRow(rowIndex) {
            const row = document.getElementById('row-' + rowIndex);
            row.classList.add('error-shake');
            setTimeout(() => row.classList.remove('error-shake'), 500);
        }

        function processGuess(guess) {
            const targetLetters = targetWord.split('');
            const guessLetters = guess.split('');
            const result = new Array(wordLength).fill('absent');
            const usedTargetIndices = new Array(wordLength).fill(false);
            
            for (let i = 0; i < wordLength; i++) {
                if (guessLetters[i] === targetLetters[i]) {
                    result[i] = 'correct';
                    usedTargetIndices[i] = true;
                }
            }
            
            for (let i = 0; i < wordLength; i++) {
                if (result[i] === 'absent') {
                    for (let j = 0; j < wordLength; j++) {
                        if (!usedTargetIndices[j] && guessLetters[i] === targetLetters[j]) {
                            result[i] = 'present';
                            usedTargetIndices[j] = true;
                            break;
                        }
                    }
                }
            }
            
            for (let i = 0; i < wordLength; i++) {
                const box = document.getElementById('box-' + currentAttempt + '-' + i);
                setTimeout(() => {
                    box.classList.remove('active-box', 'filled');
                    box.classList.add(result[i]);
                    updateKeyboard(guessLetters[i], result[i]);
                }, i * 150);
            }
        }

        function updateKeyboard(letter, state) {
            const currentState = keyboardState[letter];
            
            if (currentState === 'correct') return;
            if (currentState === 'present' && state === 'absent') return;
            
            keyboardState[letter] = state;
            
            const keyElement = document.querySelector('.key[data-key=""' + letter + '""]');
            if (keyElement) {
                keyElement.classList.remove('correct', 'present', 'absent');
                keyElement.classList.add(state);
            }
        }

        function updateAttemptsDisplay() {
            document.getElementById('attemptsLeft').textContent = maxAttempts - currentAttempt;
        }

        function endGame(won) {
            gameActive = false;
            
            const message = document.getElementById('resultMessage');
            const title = document.getElementById('resultTitle');
            const reveal = document.getElementById('answerReveal');
            
            if (won) {
                title.textContent = '🎉 Congratulations!';
                message.textContent = 'You guessed the word in ' + (currentAttempt + 1) + (currentAttempt + 1 === 1 ? ' try' : ' tries') + '!';
                message.className = 'message success';
            } else {
                title.textContent = '😔 Better Luck Next Time!';
                message.textContent = ""You've used all your attempts."";
                message.className = 'message failure';
            }
            
            reveal.innerHTML = 'The word was: <span>' + targetWord + '</span>';
            
            showPage('resultPage');
        }

        function viewPuzzle() {
            const puzzleBoard = document.getElementById('puzzleBoard');
            puzzleBoard.innerHTML = document.getElementById('gameBoard').innerHTML;
            document.getElementById('puzzleAnswer').innerHTML = 'The word was: <span>' + targetWord + '</span>';
            showPage('puzzlePage');
        }

        function backToResult() {
            showPage('resultPage');
        }

        function playAgain() {
            showSetupPage(currentGame);
        }

        function goToMenu() {
            showPage('menuPage');
        }

        function showPage(pageId) {
            const pages = document.querySelectorAll('.page');
            pages.forEach(page => page.classList.remove('active'));
            document.getElementById(pageId).classList.add('active');
        }

        document.addEventListener('keydown', (e) => {
            if (!gameActive) return;
            
            const key = e.key.toUpperCase();
            
            if (key === 'ENTER') {
                handleKeyPress('ENTER');
            } else if (key === 'BACKSPACE') {
                handleKeyPress('BACKSPACE');
            } else if (/^[A-Z]$/.test(key)) {
                handleKeyPress(key);
            }
        });
    </script>
</body>
</html>";
        }
    }
}