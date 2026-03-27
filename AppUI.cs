using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApprenticeManagement;

namespace ConsoleApp1;

public enum UserRole { Trainer, Apprentice }

public static class AppUI
{
    /// <summary>
    /// Zeigt ein modales Dialogfeld an, um eine Texteingabe vom Benutzer zu erhalten.
    /// </summary>
    /// <returns>Gibt den vom Benutzer eingegebenen Text zurück oder eine leere Zeichenfolge, wenn der Dialog abgebrochen wird.</returns>
    public static string PromptInput(string prompt, string title, string defaultValue = "", bool isPassword = false)
    {
        using Form form = new Form() { Width = 400, Height = 200, FormBorderStyle = FormBorderStyle.FixedSingle, MaximizeBox = false, Text = title, StartPosition = FormStartPosition.CenterParent, BackColor = Color.FromArgb(32, 32, 32), ForeColor = Color.White };
        Label textLabel = new Label() { Left = 20, Top = 20, Text = prompt, AutoSize = true };
        TextBox textBox = new TextBox() { Left = 20, Top = 60, Width = 340, Text = defaultValue, BackColor = Color.FromArgb(43, 43, 43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
        if (isPassword) textBox.PasswordChar = '●';
        Button confirmation = new Button() { Text = "OK", Left = 260, Top = 110, Width = 100, Height = 35, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White };
        confirmation.FlatAppearance.BorderSize = 0;
        form.Controls.Add(textLabel); form.Controls.Add(textBox); form.Controls.Add(confirmation); form.AcceptButton = confirmation;
        return form.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
    }

    /// <summary>
    /// Erstellt und konfiguriert ein DataGridView mit einem modernen, dunklen Design.
    /// </summary>
    /// <returns>Ein neues, gestyltes DataGridView-Objekt.</returns>
    public static DataGridView CreateModernGrid()
    {
        var grid = new DataGridView() {
            Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.FromArgb(45, 45, 48), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = Color.FromArgb(60, 60, 60), EnableHeadersVisualStyles = false, RowHeadersVisible = false,
            AllowUserToResizeColumns = false, AllowUserToResizeRows = false, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            RowTemplate = { Height = 40 }
        };
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(43, 43, 43), ForeColor = Color.White, SelectionBackColor = Color.FromArgb(43, 43, 43), SelectionForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), Padding = new Padding(10) };
        grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(32, 32, 32), ForeColor = Color.White, SelectionBackColor = Color.FromArgb(0, 122, 204), SelectionForeColor = Color.White, Padding = new Padding(10), WrapMode = DataGridViewTriState.True, Font = new Font("Segoe UI", 10) };
        return grid;
    }

    /// <summary>
    /// Erstellt einen Button mit einem flachen, modernen Design.
    /// </summary>
    /// <returns>Ein neues, gestyltes Button-Objekt.</returns>
    public static Button CreateFlatButton(string text, Color color)
    {
        var btn = new Button() { Text = text, Width = 160, Height = 40, FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White, Cursor = Cursors.Hand, Margin = new Padding(5) };
        btn.FlatAppearance.BorderSize = 0; return btn;
    }
}

public class JournalEntryDialog : Form
{
    /// <summary>
    /// Initialisiert ein Dialogfeld zum Hinzufügen oder Bearbeiten eines Arbeitstagebucheintrags.
    /// </summary>
    public string TaskDesc { get; private set; } = ""; public double Hours { get; private set; } = 0;
    public JournalEntryDialog(WorkJournal? existing = null) {
        Text = existing == null ? "Add Work Journal" : "Edit Work Journal"; Size = new Size(500, 420); StartPosition = FormStartPosition.CenterParent; BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
        Label lblTask = new Label() { Text = "What did you do today? (Task Description):", Left = 20, Top = 20, AutoSize = true };
        TextBox txtTask = new TextBox() { Left = 20, Top = 45, Width = 440, Height = 200, Multiline = true, BackColor = Color.FromArgb(43, 43, 43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10), ScrollBars = ScrollBars.Vertical };
        Label lblHours = new Label() { Text = "Hours Worked (e.g. 8.5):", Left = 20, Top = 270, AutoSize = true };
        TextBox txtHours = new TextBox() { Left = 20, Top = 295, Width = 150, BackColor = Color.FromArgb(43, 43, 43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
        if (existing != null) { txtTask.Text = existing.TaskDescription; txtHours.Text = existing.HoursWorked.ToString(); }
        Button btnSave = AppUI.CreateFlatButton("Save Journal", Color.FromArgb(0, 122, 204)); btnSave.Location = new Point(300, 290);
        btnSave.Click += (s, e) => { TaskDesc = txtTask.Text; if (string.IsNullOrWhiteSpace(TaskDesc)) { MessageBox.Show("Description cannot be empty."); return; } if (double.TryParse(txtHours.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double h)) { Hours = h; DialogResult = DialogResult.OK; } else { MessageBox.Show("Please enter a valid number for hours."); } };
        Controls.Add(lblTask); Controls.Add(txtTask); Controls.Add(lblHours); Controls.Add(txtHours); Controls.Add(btnSave);
    }
}

public class LoginForm : Form
{
    private ComboBox _cbRole, _cbUser; private TextBox _txtPin;
    /// <summary>
    /// Initialisiert das Anmeldefenster mit allen UI-Komponenten.
    /// </summary>
    public LoginForm() {
        Text = "AMS Login"; Size = new Size(420, 520); StartPosition = FormStartPosition.CenterScreen; BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
        Label lblBrand = new Label() { Text = "AMS Studio", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(0, 122, 204), Dock = DockStyle.Top, Height = 90, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(0, 20, 0, 0) };
        Panel pnlCenter = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(40) };
        Label lblRole = new Label() { Text = "Select Role:", AutoSize = true, Location = new Point(40, 20) };
        _cbRole = new ComboBox() { Location = new Point(40, 45), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
        _cbRole.Items.Add("Trainer (Admin)"); _cbRole.Items.Add("Apprentice"); _cbRole.SelectedIndexChanged += RoleChanged;
        Label lblUser = new Label() { Text = "Select User:", AutoSize = true, Location = new Point(40, 90) };
        _cbUser = new ComboBox() { Location = new Point(40, 115), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
        Label lblPin = new Label() { Text = "PIN Code (Default: 1234):", AutoSize = true, Location = new Point(40, 160) };
        _txtPin = new TextBox() { Location = new Point(40, 185), Width = 320, PasswordChar = '●', BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 12) };
        Button btnLogin = AppUI.CreateFlatButton("Login", Color.FromArgb(0, 122, 204)); btnLogin.Location = new Point(120, 250); btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold); btnLogin.Click += TryLogin;
        this.AcceptButton = btnLogin; pnlCenter.Controls.Add(lblRole); pnlCenter.Controls.Add(_cbRole); pnlCenter.Controls.Add(lblUser); pnlCenter.Controls.Add(_cbUser); pnlCenter.Controls.Add(lblPin); pnlCenter.Controls.Add(_txtPin); pnlCenter.Controls.Add(btnLogin); Controls.Add(pnlCenter); Controls.Add(lblBrand); _cbRole.SelectedIndex = 0;
    }
    private void RoleChanged(object? sender, EventArgs e) {
        // Aktualisiert die Benutzerliste basierend auf der ausgewählten Rolle.
        _cbUser.Items.Clear();
        if (_cbRole.SelectedIndex == 0) { if (Database.Data.Trainers.Count == 0) _cbUser.Items.Add("Default Admin"); else _cbUser.Items.AddRange(Database.Data.Trainers.Select(t => t.FullName).ToArray()); } 
        else { if (Database.Data.Apprentices.Count == 0) _cbUser.Items.Add("No Apprentices Found"); else _cbUser.Items.AddRange(Database.Data.Apprentices.Select(a => $"{a.FirstName} {a.LastName}").ToArray()); }
        if (_cbUser.Items.Count > 0) _cbUser.SelectedIndex = 0;
    }
    /// <summary>
    /// Versucht, den Benutzer basierend auf der ausgewählten Rolle, dem Namen und dem PIN-Code anzumelden.
    /// </summary>
    private void TryLogin(object? sender, EventArgs e) {
        // Überprüft die Anmeldeinformationen und öffnet das Hauptformular bei Erfolg.
        UserRole role = _cbRole.SelectedIndex == 0 ? UserRole.Trainer : UserRole.Apprentice; Apprentice? loggedInApprentice = null; VocationalTrainer? loggedInTrainer = null;
        string selectedName = _cbUser.SelectedItem?.ToString() ?? ""; string enteredPin = _txtPin.Text;
        if (role == UserRole.Trainer && selectedName != "Default Admin") { loggedInTrainer = Database.Data.Trainers.FirstOrDefault(t => t.FullName == selectedName); if (loggedInTrainer == null || loggedInTrainer.PinCode != enteredPin) { MessageBox.Show("Invalid PIN."); return; } } 
        else if (role == UserRole.Trainer && selectedName == "Default Admin" && enteredPin != "1234") { MessageBox.Show("Invalid Default PIN."); return; } 
        else if (role == UserRole.Apprentice) { loggedInApprentice = Database.Data.Apprentices.FirstOrDefault(a => $"{a.FirstName} {a.LastName}" == selectedName); if (loggedInApprentice == null || loggedInApprentice.PinCode != enteredPin) { MessageBox.Show("Invalid PIN."); return; } }
        MainForm mainForm = new MainForm(role, loggedInApprentice, loggedInTrainer); mainForm.FormClosed += (s, args) => this.Close(); this.Hide(); mainForm.Show();
    }
}

// ==========================================
// ADVANCED BLACKJACK PANEL
// ==========================================
public class BlackjackPanel : Panel
{
    private Label lblCredits, lblDealerCards, lblPlayerCards, lblMessage;
    private TextBox txtBet; private Button btnDeal, btnHit, btnStand, btnDouble, btnSplit;
    private int _currentCredits, _currentBet, _splitBet;
    private bool _isDoubled = false, _playingSplit = false;
    private List<string> _playerHand = new List<string>(), _dealerHand = new List<string>(), _deck = new List<string>(), _splitHand = null;
    private Random _rng = new Random(); private Action<int> _saveCredits;

    /// <summary>
    /// Initialisiert das Blackjack-Spielpanel mit allen UI-Elementen und der Spiellogik.
    /// </summary>
    public BlackjackPanel(int initialCredits, Action<int> saveCreditsCallback)
    {
        _currentCredits = initialCredits; _saveCredits = saveCreditsCallback; Dock = DockStyle.Fill;
        Label lblTitle = new Label() { Text = "🎲 Casino: Blackjack", Font = new Font("Segoe UI", 24, FontStyle.Bold), Dock = DockStyle.Top, Height = 75, Padding = new Padding(0, 15, 0, 0) };
        lblCredits = new Label() { Text = $"Credits: {_currentCredits}", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.Gold, AutoSize = true, Location = new Point(20, 90) };
        Label lblBet = new Label() { Text = "Bet Amount:", AutoSize = true, Location = new Point(20, 135), Font = new Font("Segoe UI", 12) };
        txtBet = new TextBox() { Location = new Point(140, 135), Width = 100, Text = "10", BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, Font = new Font("Segoe UI", 12) };

        btnDeal = AppUI.CreateFlatButton("Deal", Color.FromArgb(0, 122, 204)); btnDeal.Location = new Point(260, 130); btnDeal.Click += BtnDeal_Click;
        btnHit = AppUI.CreateFlatButton("Hit", Color.MediumSeaGreen); btnHit.Location = new Point(430, 130); btnHit.Enabled = false; btnHit.Click += BtnHit_Click;
        btnStand = AppUI.CreateFlatButton("Stand", Color.IndianRed); btnStand.Location = new Point(600, 130); btnStand.Enabled = false; btnStand.Click += BtnStand_Click;
        btnDouble = AppUI.CreateFlatButton("Double", Color.Orange); btnDouble.Location = new Point(770, 130); btnDouble.Enabled = false; btnDouble.Click += BtnDouble_Click;
        btnSplit = AppUI.CreateFlatButton("Split", Color.Plum); btnSplit.Location = new Point(940, 130); btnSplit.Enabled = false; btnSplit.Click += BtnSplit_Click;

        Label lblDealerTitle = new Label() { Text = "Dealer's Hand:", AutoSize = true, Location = new Point(20, 200), Font = new Font("Segoe UI", 14, FontStyle.Bold) };
        lblDealerCards = new Label() { Text = "-", AutoSize = true, Location = new Point(20, 230), Font = new Font("Segoe UI", 20) };
        Label lblPlayerTitle = new Label() { Text = "Your Hand:", AutoSize = true, Location = new Point(20, 300), Font = new Font("Segoe UI", 14, FontStyle.Bold) };
        lblPlayerCards = new Label() { Text = "-", AutoSize = true, Location = new Point(20, 330), Font = new Font("Segoe UI", 20), MaximumSize = new Size(1000, 150) };
        lblMessage = new Label() { Text = "Place your bet and click Deal!", AutoSize = true, Location = new Point(20, 450), Font = new Font("Segoe UI", 16, FontStyle.Italic), ForeColor = Color.LightSkyBlue };

        Controls.AddRange(new Control[] { lblTitle, lblCredits, lblBet, txtBet, btnDeal, btnHit, btnStand, btnDouble, btnSplit, lblDealerTitle, lblDealerCards, lblPlayerTitle, lblPlayerCards, lblMessage });
    }

    /// <summary>
    /// Erstellt ein neues Kartendeck und mischt es.
    /// </summary>
    private void GenerateDeck() { _deck.Clear(); string[] suits = { "♠", "♥", "♦", "♣" }; string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" }; foreach (var s in suits) foreach (var r in ranks) _deck.Add(r + s); _deck = _deck.OrderBy(x => _rng.Next()).ToList(); }
    
    /// <summary>
    /// Zieht die oberste Karte vom Deck.
    /// </summary>
    private string DrawCard() { var card = _deck[0]; _deck.RemoveAt(0); return card; }
    
    /// <summary>
    /// Berechnet den Punktwert einer Hand.
    /// </summary>
    private int CalculateScore(List<string> hand) { 
        int score = 0; int aces = 0; 
        foreach (var card in hand) { 
            string rank = card.Substring(0, card.Length - 1); 
            if (rank == "J" || rank == "Q" || rank == "K") score += 10; 
            else if (rank == "A") { score += 11; aces++; } 
            else score += int.Parse(rank); 
        } 
        while (score > 21 && aces > 0) { score -= 10; aces--; } // Auto Ace logic for both Player & Dealer
        return score; 
    }

    /// <summary>
    /// Prüft, ob die Hand des Spielers geteilt werden kann.
    /// </summary>
    private bool CanSplit() {
        if (_playerHand.Count != 2) return false;
        string r1 = _playerHand[0].Substring(0, _playerHand[0].Length - 1); string r2 = _playerHand[1].Substring(0, _playerHand[1].Length - 1);
        if (r1 == "J" || r1 == "Q" || r1 == "K") r1 = "10"; if (r2 == "J" || r2 == "Q" || r2 == "K") r2 = "10";
        return r1 == r2;
    }

    /// <summary>
    /// Aktualisiert die Benutzeroberfläche, um die aktuellen Hände und Punktzahlen anzuzeigen.
    /// </summary>
    private void UpdateUI() {
        if (_dealerHand.Count > 0) lblDealerCards.Text = _dealerHand[0] + "  [Hidden]";
        string pText = string.Join("  ", _playerHand) + $"  (Score: {CalculateScore(_playerHand)})";
        if (_splitHand != null) {
            string sText = string.Join("  ", _splitHand) + $"  (Score: {CalculateScore(_splitHand)})";
            lblPlayerCards.Text = (!_playingSplit ? "👉 " : "   ") + $"Hand 1: {pText}\n" + (_playingSplit ? "👉 " : "   ") + $"Hand 2: {sText}";
        } else { lblPlayerCards.Text = pText; }
    }

    /// <summary>
    /// Startet eine neue Spielrunde, wenn auf "Deal" geklickt wird.
    /// </summary>
    private void BtnDeal_Click(object sender, EventArgs e) {
        if (!int.TryParse(txtBet.Text, out _currentBet) || _currentBet <= 0) { MessageBox.Show("Invalid bet."); return; }
        if (_currentBet > _currentCredits) { MessageBox.Show("Not enough credits!"); return; }
        _isDoubled = false; _playingSplit = false; _splitHand = null;
        _currentCredits -= _currentBet; _saveCredits(_currentCredits); UpdateCreditsLabel();
        GenerateDeck(); _playerHand.Clear(); _dealerHand.Clear(); 
        
        _playerHand.Add(DrawCard()); _dealerHand.Add(DrawCard()); _playerHand.Add(DrawCard()); _dealerHand.Add(DrawCard());
        
        btnDeal.Enabled = false; txtBet.Enabled = false; btnHit.Enabled = true; btnStand.Enabled = true;
        btnDouble.Enabled = _currentCredits >= _currentBet; btnSplit.Enabled = CanSplit() && _currentCredits >= _currentBet;
        lblMessage.Text = "Hit, Stand, Double, or Split?"; 
        UpdateUI(); 
        if (CalculateScore(_playerHand) >= 21) BtnStand_Click(null, null); 
    }

    /// <summary>
    /// Zieht eine weitere Karte für die aktive Hand des Spielers.
    /// </summary>
    private void BtnHit_Click(object sender, EventArgs e) { 
        btnDouble.Enabled = false; btnSplit.Enabled = false;
        var activeHand = _playingSplit ? _splitHand : _playerHand;
        activeHand.Add(DrawCard()); UpdateUI(); 
        if (CalculateScore(activeHand) >= 21) BtnStand_Click(null, null); 
    }

    /// <summary>
    /// Verdoppelt den Einsatz, zieht eine Karte und beendet den Zug.
    /// </summary>
    private void BtnDouble_Click(object sender, EventArgs e) {
        _isDoubled = true;
        _currentCredits -= _currentBet; _currentBet *= 2; _saveCredits(_currentCredits); UpdateCreditsLabel();
        btnDouble.Enabled = false; btnSplit.Enabled = false;
        var activeHand = _playingSplit ? _splitHand : _playerHand;
        activeHand.Add(DrawCard()); UpdateUI(); BtnStand_Click(null, null);
    }

    /// <summary>
    /// Teilt die Hand des Spielers in zwei separate Hände auf.
    /// </summary>
    private void BtnSplit_Click(object sender, EventArgs e) {
        _currentCredits -= _currentBet; _splitBet = _currentBet; _saveCredits(_currentCredits); UpdateCreditsLabel();
        _splitHand = new List<string> { _playerHand[1] }; _playerHand.RemoveAt(1);
        _playerHand.Add(DrawCard()); _splitHand.Add(DrawCard());
        btnSplit.Enabled = false; btnDouble.Enabled = false; UpdateUI();
        if (CalculateScore(_playerHand) >= 21) BtnStand_Click(null, null);
    }

    /// <summary>
    /// Beendet den Zug für die aktuelle Hand oder das gesamte Spiel.
    /// </summary>
    private void BtnStand_Click(object sender, EventArgs e) {
        if (_splitHand != null && !_playingSplit) { _playingSplit = true; UpdateUI(); if (CalculateScore(_splitHand) >= 21) BtnStand_Click(null, null); return; }
        EndGame();
    }

    /// <summary>
    /// Beendet das Spiel, deckt die Karten des Dealers auf und ermittelt den Gewinner.
    /// </summary>
    private void EndGame() {
        btnHit.Enabled = false; btnStand.Enabled = false; btnDouble.Enabled = false; btnSplit.Enabled = false;
        while (CalculateScore(_dealerHand) < 17) _dealerHand.Add(DrawCard());
        int dScore = CalculateScore(_dealerHand); lblDealerCards.Text = string.Join("  ", _dealerHand) + $"  (Score: {dScore})";
        
        int winnings = 0; 
        int pScore1 = CalculateScore(_playerHand); 
        winnings += ResolveHand(pScore1, dScore, _currentBet, _isDoubled);
        
        if (_splitHand != null) { 
            int pScore2 = CalculateScore(_splitHand); 
            winnings += ResolveHand(pScore2, dScore, _splitBet, false); 
        }
        
        _currentCredits += winnings; _saveCredits(_currentCredits); UpdateCreditsLabel(); btnDeal.Enabled = true; txtBet.Enabled = true;
        if (winnings > 0) { lblMessage.Text = $"You Win! (+{winnings} Credits)"; lblMessage.ForeColor = Color.MediumSeaGreen; } 
        else if (winnings == 0 && (pScore1 == dScore || (_splitHand != null && CalculateScore(_splitHand) == dScore))) { lblMessage.Text = "Push (Tie)."; lblMessage.ForeColor = Color.Gold; } 
        else { lblMessage.Text = "Dealer Wins."; lblMessage.ForeColor = Color.IndianRed; }
    }

    /// <summary>
    /// Bestimmt den Ausgang für eine einzelne Hand und gibt den Gewinn zurück.
    /// </summary>
    private int ResolveHand(int pScore, int dScore, int bet, bool isDoubled) {
        if (pScore > 21) return 0;
        if (pScore == 21) return isDoubled ? bet * 8 : bet * 4; // 8x Multiplier if Doubled and hit 21!
        if (dScore > 21 || pScore > dScore) return bet * 2;
        if (pScore == dScore) return bet;
        return 0;
    }

    /// <summary>
    /// Aktualisiert die Anzeige der Credits.
    /// </summary>
    private void UpdateCreditsLabel() => lblCredits.Text = $"Credits: {_currentCredits}";
    /// <summary>
    /// Synchronisiert die Credits des Spiels mit dem externen Wert.
    /// </summary>
    public void SyncCredits(int newCredits) { _currentCredits = newCredits; UpdateCreditsLabel(); }
}

public class MainForm : Form
{
    private Panel _mainPanel; private DataGridView _apprenticeGrid, _companyGrid, _trainerGrid;
    private Panel _pnlHome, _pnlApprentices, _pnlCompanies, _pnlTrainers, _pnlSettings, _pnlStatistics;
    private BlackjackPanel _pnlBlackjack; private Label _lblGlobalStats; private FlowLayoutPanel _flpStatistics;
    private Stack<Panel> _navHistory = new Stack<Panel>(); private Panel _currentView = null!;
    private UserRole _role; private Apprentice? _loggedInApprentice; private VocationalTrainer? _loggedInTrainer;

    /// <summary>
    /// Initialisiert das Hauptformular der Anwendung, einschliesslich Seitenleiste und Hauptinhaltsbereich.
    /// </summary>
    public MainForm(UserRole role, Apprentice? loggedInApprentice, VocationalTrainer? loggedInTrainer = null)
    {
        _role = role; _loggedInApprentice = loggedInApprentice; _loggedInTrainer = loggedInTrainer;
        Text = "AMS - Modern Dashboard"; Size = new Size(1200, 800); StartPosition = FormStartPosition.CenterScreen; BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White; MinimumSize = new Size(1000, 700);

        FlowLayoutPanel sidebar = new FlowLayoutPanel() { Dock = DockStyle.Left, Width = 220, BackColor = Color.FromArgb(25, 25, 25), Padding = new Padding(10, 20, 10, 10), FlowDirection = FlowDirection.TopDown };
        Label lblBrand = new Label() { Text = "AMS Studio", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(0, 122, 204), Margin = new Padding(10, 0, 0, 30) };
        sidebar.Controls.Add(lblBrand); _mainPanel = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(20) }; Controls.Add(_mainPanel); Controls.Add(sidebar);

        int startCredits = _role == UserRole.Trainer ? (_loggedInTrainer?.CasinoCredits ?? 1000) : (_loggedInApprentice?.CasinoCredits ?? 1000);
        _pnlBlackjack = new BlackjackPanel(startCredits, (newCredits) => { if (_role == UserRole.Trainer && _loggedInTrainer != null) _loggedInTrainer.CasinoCredits = newCredits; else if (_role == UserRole.Apprentice && _loggedInApprentice != null) _loggedInApprentice.CasinoCredits = newCredits; Database.Save(); });

        if (_role == UserRole.Trainer) {
            Button btnNavHome = AppUI.CreateFlatButton("🏠 Overview", Color.FromArgb(45, 45, 48)); btnNavHome.Width = 200; btnNavHome.Click += (s, e) => SwitchSidebarView(_pnlHome);
            Button btnNavApp = AppUI.CreateFlatButton("👨‍🎓 Apprentices", Color.FromArgb(45, 45, 48)); btnNavApp.Width = 200; btnNavApp.Click += (s, e) => SwitchSidebarView(_pnlApprentices);
            Button btnNavComp = AppUI.CreateFlatButton("🏢 Companies", Color.FromArgb(45, 45, 48)); btnNavComp.Width = 200; btnNavComp.Click += (s, e) => SwitchSidebarView(_pnlCompanies);
            Button btnNavTrain = AppUI.CreateFlatButton("👨‍🏫 Trainers", Color.FromArgb(45, 45, 48)); btnNavTrain.Width = 200; btnNavTrain.Click += (s, e) => SwitchSidebarView(_pnlTrainers);
            Button btnNavStats = AppUI.CreateFlatButton("📊 Statistics", Color.FromArgb(45, 45, 48)); btnNavStats.Width = 200; btnNavStats.Click += (s, e) => SwitchSidebarView(_pnlStatistics);
            Button btnNavBJ = AppUI.CreateFlatButton("🎲 Blackjack", Color.FromArgb(100, 50, 150)); btnNavBJ.Width = 200; btnNavBJ.Click += (s, e) => SwitchSidebarView(_pnlBlackjack);
            Panel spacer = new Panel() { Width = 200, Height = 20, BackColor = Color.Transparent };
            Button btnNavSet = AppUI.CreateFlatButton("⚙️ Settings", Color.FromArgb(45, 45, 48)); btnNavSet.Width = 200; btnNavSet.Click += (s, e) => SwitchSidebarView(_pnlSettings);
            Button btnLogout = AppUI.CreateFlatButton("🚪 Logout", Color.FromArgb(180, 50, 50)); btnLogout.Width = 200; btnLogout.Click += (s, e) => Application.Restart();
            sidebar.Controls.AddRange(new Control[] { btnNavHome, btnNavApp, btnNavComp, btnNavTrain, btnNavStats, btnNavBJ, spacer, btnNavSet, btnLogout });
            _pnlHome = CreateHomePanel(); _pnlSettings = CreateSettingsPanel(); _pnlStatistics = CreateStatisticsPanel();
            _apprenticeGrid = AppUI.CreateModernGrid(); _apprenticeGrid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0 && _apprenticeGrid.Rows[e.RowIndex].DataBoundItem is Apprentice app) NavigateForward(CreateDashboardPanel(app)); };
            _pnlApprentices = CreateApprenticeDirectoryPanel();
            _companyGrid = AppUI.CreateModernGrid(); _pnlCompanies = CreateBaseViewPanel("Company Directory", _companyGrid, AddCompany, DeleteCompany, EditCompany, SearchCompanies);
            _trainerGrid = AppUI.CreateModernGrid(); _pnlTrainers = CreateBaseViewPanel("Trainer Directory", _trainerGrid, AddTrainer, DeleteTrainer, EditTrainer, SearchTrainers);
            SwitchSidebarView(_pnlHome);
        } else if (_role == UserRole.Apprentice && _loggedInApprentice != null) {
            Button btnMyDash = AppUI.CreateFlatButton("👤 My Dashboard", Color.FromArgb(0, 122, 204)); btnMyDash.Width = 200; btnMyDash.Click += (s, e) => { _navHistory.Clear(); SwitchSidebarView(CreateDashboardPanel(_loggedInApprentice)); };
            Button btnNavBJ = AppUI.CreateFlatButton("🎲 Blackjack", Color.FromArgb(100, 50, 150)); btnNavBJ.Width = 200; btnNavBJ.Click += (s, e) => SwitchSidebarView(_pnlBlackjack);
            Panel spacerApp = new Panel() { Width = 200, Height = 400, BackColor = Color.Transparent };
            Button btnLogoutApp = AppUI.CreateFlatButton("🚪 Logout", Color.FromArgb(180, 50, 50)); btnLogoutApp.Width = 200; btnLogoutApp.Click += (s, e) => Application.Restart();
            sidebar.Controls.AddRange(new Control[] { btnMyDash, btnNavBJ, spacerApp, btnLogoutApp });
            SwitchSidebarView(CreateDashboardPanel(_loggedInApprentice));
        }
    }

    /// <summary>
    /// Wechselt die im Hauptbereich angezeigte Ansicht und setzt den Navigationsverlauf zurück.
    /// </summary>
    private void SwitchSidebarView(Panel baseView) { 
        _navHistory.Clear(); foreach (Control c in _mainPanel.Controls) c.Visible = false; _currentView = baseView; 
        if (!_mainPanel.Controls.Contains(baseView)) _mainPanel.Controls.Add(baseView); 
        if (baseView == _pnlBlackjack) { int currentC = _role == UserRole.Trainer ? (_loggedInTrainer?.CasinoCredits ?? 1000) : (_loggedInApprentice?.CasinoCredits ?? 1000); _pnlBlackjack.SyncCredits(currentC); }
        baseView.Visible = true; baseView.BringToFront(); RefreshGrids(); UpdateHomeStats(); if (baseView == _pnlStatistics) UpdateStatisticsPanel();
    }
    
    /// <summary>
    /// Navigiert zu einer neuen Ansicht und speichert die aktuelle Ansicht im Verlauf.
    /// </summary>
    private void NavigateForward(Panel newView) { _navHistory.Push(_currentView); _currentView.Visible = false; newView.Dock = DockStyle.Fill; _mainPanel.Controls.Add(newView); _currentView = newView; _currentView.Visible = true; _currentView.BringToFront(); }
    /// <summary>
    /// Navigiert zur vorherigen Ansicht im Verlauf zurück.
    /// </summary>
    private void NavigateBack() { if (_navHistory.Count > 0) { _mainPanel.Controls.Remove(_currentView); _currentView.Dispose(); _currentView = _navHistory.Pop(); if (!_mainPanel.Controls.Contains(_currentView)) _mainPanel.Controls.Add(_currentView); _currentView.Visible = true; _currentView.BringToFront(); RefreshGrids(); UpdateHomeStats(); } }
    /// <summary>
    /// Ersetzt die aktuelle Ansicht durch eine neue, ohne den Navigationsverlauf zu ändern.
    /// </summary>
    private void ReplaceCurrentView(Panel newView) { _mainPanel.Controls.Remove(_currentView); _currentView.Dispose(); _currentView = newView; newView.Dock = DockStyle.Fill; _mainPanel.Controls.Add(newView); newView.BringToFront(); RefreshGrids(); }

    /// <summary>
    /// Filtert die Lernendenliste basierend auf dem Suchtext.
    /// </summary>
    private void SearchApprentices(object? sender, EventArgs e) { if(sender is TextBox t) _apprenticeGrid.DataSource = Database.Data.Apprentices.Where(a => a.FirstName.Contains(t.Text, StringComparison.OrdinalIgnoreCase) || a.LastName.Contains(t.Text, StringComparison.OrdinalIgnoreCase)).ToList(); }
    /// <summary>
    /// Filtert die Firmenliste basierend auf dem Suchtext.
    /// </summary>
    private void SearchCompanies(object? sender, EventArgs e) { if(sender is TextBox t) _companyGrid.DataSource = Database.Data.Companies.Where(c => c.Name.Contains(t.Text, StringComparison.OrdinalIgnoreCase)).ToList(); }
    /// <summary>
    /// Filtert die Ausbilderliste basierend auf dem Suchtext.
    /// </summary>
    private void SearchTrainers(object? sender, EventArgs e) { if(sender is TextBox t) _trainerGrid.DataSource = Database.Data.Trainers.Where(tr => tr.FirstName.Contains(t.Text, StringComparison.OrdinalIgnoreCase) || tr.LastName.Contains(t.Text, StringComparison.OrdinalIgnoreCase)).ToList(); }

    /// <summary>
    /// Erstellt das "Home"-Panel, das eine Systemübersicht anzeigt.
    /// </summary>
    private Panel CreateHomePanel() { Panel pnl = new Panel() { Dock = DockStyle.Fill }; Label lblTitle = new Label() { Text = "System Overview", Font = new Font("Segoe UI", 24, FontStyle.Bold), Dock = DockStyle.Top, Height = 75, Padding = new Padding(0, 15, 0, 0) }; _lblGlobalStats = new Label() { Font = new Font("Segoe UI", 16), Dock = DockStyle.Fill, Padding = new Padding(20) }; pnl.Controls.Add(_lblGlobalStats); pnl.Controls.Add(lblTitle); return pnl; }

    /// <summary>
    /// Aktualisiert die globalen Statistiken auf dem "Home"-Panel.
    /// </summary>
    private void UpdateHomeStats() {
        if (_lblGlobalStats == null || _role != UserRole.Trainer) return; var apps = Database.Data.Apprentices;
        double globalGpa = apps.Any(a => a.OverallGPA > 0) ? apps.Where(a => a.OverallGPA > 0).Average(a => a.OverallGPA) : 0;
        int atRisk = apps.Count(a => a.IsAtRisk);
        _lblGlobalStats.Text = $"📊 Total Apprentices: {apps.Count}\n🏢 Total Companies: {Database.Data.Companies.Count}\n👨‍🏫 Total Trainers: {Database.Data.Trainers.Count}\n\n🎓 Global Average GPA: {globalGpa:F2}\n⚠️ Apprentices At Risk (GPA < 4.0 or Sick > 15): {atRisk}";
    }

    /// <summary>
    /// Erstellt das Panel zur Anzeige detaillierter Statistiken.
    /// </summary>
    private Panel CreateStatisticsPanel() { Panel pnl = new Panel() { Dock = DockStyle.Fill }; Label lblTitle = new Label() { Text = "Detailed Statistics", Font = new Font("Segoe UI", 24, FontStyle.Bold), Dock = DockStyle.Top, Height = 75, Padding = new Padding(0, 15, 0, 0) }; _flpStatistics = new FlowLayoutPanel() { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true }; pnl.Controls.Add(_flpStatistics); pnl.Controls.Add(lblTitle); return pnl; }

    private void UpdateStatisticsPanel() {
        if (_flpStatistics == null) return; _flpStatistics.Controls.Clear(); var apps = Database.Data.Apprentices;
        if (apps.Count == 0) { _flpStatistics.Controls.Add(new Label { Text = "Not enough data to display statistics.", ForeColor = Color.Gray, AutoSize = true, Font = new Font("Segoe UI", 14) }); return; }
        double globalGpa = apps.Any(a => a.OverallGPA > 0) ? apps.Where(a => a.OverallGPA > 0).Average(a => a.OverallGPA) : 0;
        int totalSick = apps.Sum(a => a.SickDays); int totalJournals = apps.Sum(a => a.WorkJournals.Count);
        int totalCasinoCoins = apps.Sum(a => a.CasinoCredits) + Database.Data.Trainers.Sum(t => t.CasinoCredits);
        var deptGroups = apps.GroupBy(a => a.Department).OrderByDescending(g => g.Count());
        string topDept = deptGroups.Any() ? $"{deptGroups.First().Key} ({deptGroups.First().Count()})" : "N/A";
        int year1 = apps.Count(a => a.CurrentYear == 1); int year2 = apps.Count(a => a.CurrentYear == 2); int year3 = apps.Count(a => a.CurrentYear == 3); int year4 = apps.Count(a => a.CurrentYear == 4);

        _flpStatistics.Controls.Add(CreateStatCard("Global Average GPA", $"{globalGpa:F2}", Color.MediumSeaGreen));
        _flpStatistics.Controls.Add(CreateStatCard("Total Work Journals", $"{totalJournals}", Color.LightSkyBlue));
        _flpStatistics.Controls.Add(CreateStatCard("Total Sick Days (Company)", $"{totalSick}", Color.IndianRed));
        _flpStatistics.Controls.Add(CreateStatCard("Most Popular Dept.", topDept, Color.Plum));
        _flpStatistics.Controls.Add(CreateStatCard("Apprentices (Year 1 & 2)", $"Y1: {year1} | Y2: {year2}", Color.Gold));
        _flpStatistics.Controls.Add(CreateStatCard("Apprentices (Year 3 & 4)", $"Y3: {year3} | Y4: {year4}", Color.Gold));
        _flpStatistics.Controls.Add(CreateStatCard("Casino Economy", $"{totalCasinoCoins} Coins", Color.Orange));
    }

    /// <summary>
    /// Erstellt eine einzelne "Statistikkarte" zur Anzeige eines Kennwerts.
    /// </summary>
    private Panel CreateStatCard(string title, string value, Color valueColor) { Panel card = new Panel() { Width = 300, Height = 150, BackColor = Color.FromArgb(45, 45, 48), Margin = new Padding(15) }; Label lblTitle = new Label() { Text = title, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.LightGray, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(0, 10, 0, 0) }; Label lblValue = new Label() { Text = value, Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = valueColor, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }; card.Controls.Add(lblValue); card.Controls.Add(lblTitle); return card; }

    /// <summary>
    /// Erstellt das Einstellungs-Panel, z.B. für Backups.
    /// </summary>
    private Panel CreateSettingsPanel() { Panel pnl = new Panel() { Dock = DockStyle.Fill }; Label lblTitle = new Label() { Text = "System Settings", Font = new Font("Segoe UI", 24, FontStyle.Bold), Dock = DockStyle.Top, Height = 75, Padding = new Padding(0, 15, 0, 0) }; Button btnBackup = AppUI.CreateFlatButton("💾 Backup JSON Database", Color.FromArgb(40, 167, 69)); btnBackup.Width = 250; btnBackup.Location = new Point(20, 100); btnBackup.Click += (s, e) => { using SaveFileDialog sfd = new SaveFileDialog() { Filter = "JSON files (*.json)|*.json", FileName = $"AMS_Backup_{DateTime.Now:yyyyMMdd}.json" }; if (sfd.ShowDialog() == DialogResult.OK) { File.Copy("app_data.json", sfd.FileName, true); MessageBox.Show("Backup created!"); } }; pnl.Controls.Add(btnBackup); pnl.Controls.Add(lblTitle); return pnl; }

    /// <summary>
    /// Erstellt das Panel für das Verzeichnis der Lernenden.
    /// </summary>
    private Panel CreateApprenticeDirectoryPanel() {
        Panel pnl = new Panel() { Dock = DockStyle.Fill, Visible = false }; Label lblTitle = new Label() { Text = "Apprentice Directory", Font = new Font("Segoe UI", 20, FontStyle.Bold), Dock = DockStyle.Top, Height = 60, Padding = new Padding(0, 10, 0, 0) };
        FlowLayoutPanel toolbar = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 60 };
        Button btnAdd = AppUI.CreateFlatButton("➕ Add New", Color.FromArgb(0, 122, 204)); btnAdd.Click += (s, e) => AddApprentice();
        Button btnEdit = AppUI.CreateFlatButton("✏️ Edit Selected", Color.FromArgb(63, 63, 70)); btnEdit.Click += (s, e) => EditApprentice();
        Button btnDelete = AppUI.CreateFlatButton("🗑️ Delete Selected", Color.FromArgb(180, 50, 50)); btnDelete.Click += (s, e) => DeleteApprentice();
        Button btnExportCsv = AppUI.CreateFlatButton("📊 Export to CSV", Color.FromArgb(40, 167, 69)); btnExportCsv.Click += (s, e) => ExportDirectoryToCsv();
        TextBox txtSearch = new TextBox() { Width = 200, Margin = new Padding(20, 12, 0, 0), BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) }; txtSearch.TextChanged += SearchApprentices;
        Label lblSearch = new Label() { Text = "🔍 Search:", Margin = new Padding(20, 15, 0, 0), AutoSize = true, ForeColor = Color.Gray };
        toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnExportCsv, lblSearch, txtSearch });
        pnl.Controls.Add(_apprenticeGrid); pnl.Controls.Add(toolbar); pnl.Controls.Add(lblTitle); _apprenticeGrid.BringToFront(); return pnl;
    }

    /// <summary>
    /// Exportiert die aktuelle Liste der Lernenden in eine CSV-Datei.
    /// </summary>
    private void ExportDirectoryToCsv() { using SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV files (*.csv)|*.csv", FileName = $"Apprentice_Export_{DateTime.Now:yyyyMMdd}.csv" }; if (sfd.ShowDialog() == DialogResult.OK) { StringBuilder csv = new StringBuilder(); csv.AppendLine("Status,First Name,Last Name,Department,Company,Trainer,Year,Sick Days,GPA"); foreach(var app in Database.Data.Apprentices) { csv.AppendLine($"{app.Status},{app.FirstName},{app.LastName},{app.Department},{app.CompanyName},{app.TrainerName},{app.CurrentYear},{app.SickDays},{app.OverallGPA:F2}"); } File.WriteAllText(sfd.FileName, csv.ToString()); MessageBox.Show("CSV Exported Successfully!"); } }

    /// <summary>
    /// Erstellt ein generisches Verzeichnis-Panel mit Standard-Aktionen (Hinzufügen, Bearbeiten, Löschen, Suchen).
    /// </summary>
    private Panel CreateBaseViewPanel(string title, DataGridView grid, Action onAdd, Action onDelete, Action onEdit, EventHandler onSearch) {
        Panel pnl = new Panel() { Dock = DockStyle.Fill, Visible = false }; Label lblTitle = new Label() { Text = title, Font = new Font("Segoe UI", 20, FontStyle.Bold), Dock = DockStyle.Top, Height = 60, Padding = new Padding(0, 10, 0, 0) }; FlowLayoutPanel toolbar = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 60 };
        Button btnAdd = AppUI.CreateFlatButton("➕ Add New", Color.FromArgb(0, 122, 204)); btnAdd.Click += (s, e) => onAdd();
        Button btnEdit = AppUI.CreateFlatButton("✏️ Edit Selected", Color.FromArgb(63, 63, 70)); btnEdit.Click += (s, e) => onEdit();
        Button btnDelete = AppUI.CreateFlatButton("🗑️ Delete Selected", Color.FromArgb(180, 50, 50)); btnDelete.Click += (s, e) => onDelete();
        TextBox txtSearch = new TextBox() { Width = 200, Margin = new Padding(20, 12, 0, 0), BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) }; txtSearch.TextChanged += onSearch;
        Label lblSearch = new Label() { Text = "🔍 Search:", Margin = new Padding(20, 15, 0, 0), AutoSize = true, ForeColor = Color.Gray };
        toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, lblSearch, txtSearch }); pnl.Controls.Add(grid); pnl.Controls.Add(toolbar); pnl.Controls.Add(lblTitle); grid.BringToFront(); return pnl;
    }

    /// <summary>
    /// Erstellt das persönliche Dashboard-Panel für einen Lernenden.
    /// </summary>
    private Panel CreateDashboardPanel(Apprentice app) {
        Panel pnl = new Panel() { Dock = DockStyle.Fill };
        if (_navHistory.Count > 0) { Panel toolbar = new Panel() { Dock = DockStyle.Top, Height = 70 }; Button btnBack = AppUI.CreateFlatButton("⬅ Back", Color.FromArgb(80, 80, 80)); btnBack.Location = new Point(0, 15); btnBack.Click += (s, e) => NavigateBack(); Label lblTitle = new Label() { Text = $"Dashboard: {app.FirstName} {app.LastName}", Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = true, Location = new Point(170, 18) }; toolbar.Controls.Add(btnBack); toolbar.Controls.Add(lblTitle); pnl.Controls.Add(toolbar); } 
        else { Label lblTitle = new Label() { Text = $"My Dashboard: {app.FirstName} {app.LastName}", Font = new Font("Segoe UI", 24, FontStyle.Bold), Dock = DockStyle.Top, Height = 75, Padding = new Padding(0, 15, 0, 0) }; pnl.Controls.Add(lblTitle); }

        Label lblStats = new Label() { Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 13), Text = $"Status: {app.Status}  |  Company: {app.CompanyName}  |  Trainer: {app.TrainerName}\nGPA: {app.OverallGPA:F2}  |  Year: {app.CurrentYear}  |  Sick Days: {app.SickDays}  |  Attendance: {app.AttendanceRate}%", Padding = new Padding(0, 0, 0, 10) };

        TableLayoutPanel splitContainer = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(0, 10, 0, 0) }; splitContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); splitContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        TableLayoutPanel actionGrid = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 }; actionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); actionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F)); actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F)); actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

        int currentRow = 0;
        if (_role == UserRole.Trainer) {
            Button btnEdit = AppUI.CreateFlatButton("✏️ Edit Profile", Color.FromArgb(63, 63, 70)); btnEdit.Dock = DockStyle.Fill; btnEdit.Click += (s, e) => { EditApprenticeDialog dialog = new EditApprenticeDialog(app); if (dialog.ShowDialog() == DialogResult.OK) { ReplaceCurrentView(CreateDashboardPanel(app)); } };
            Button btnPromote = AppUI.CreateFlatButton("🌟 Promote Year", Color.FromArgb(100, 50, 150)); btnPromote.Dock = DockStyle.Fill; btnPromote.Click += (s, e) => { if(app.CurrentYear < 4) { app.CurrentYear++; Database.Save(); ReplaceCurrentView(CreateDashboardPanel(app)); } };
            actionGrid.Controls.Add(btnEdit, 0, currentRow); actionGrid.Controls.Add(btnPromote, 1, currentRow); currentRow++;
        }

        Button btnSubjects = AppUI.CreateFlatButton("📚 Subjects & Exams", Color.FromArgb(63, 63, 70)); btnSubjects.Dock = DockStyle.Fill; btnSubjects.Click += (s, e) => NavigateForward(CreateSubjectsPanel(app));
        Button btnJournal = AppUI.CreateFlatButton("📝 Work Journals", Color.FromArgb(63, 63, 70)); btnJournal.Dock = DockStyle.Fill; btnJournal.Click += (s, e) => NavigateForward(CreateJournalsPanel(app));
        actionGrid.Controls.Add(btnSubjects, 0, currentRow); actionGrid.Controls.Add(btnJournal, 1, currentRow); currentRow++;
        
        Button btnSick = AppUI.CreateFlatButton("🤒 Log Sick Day", Color.FromArgb(63, 63, 70)); btnSick.Dock = DockStyle.Fill; btnSick.Click += (s, e) => { app.SickDays++; Database.Save(); ReplaceCurrentView(CreateDashboardPanel(app)); };
        Button btnExport = AppUI.CreateFlatButton("📄 Export txt Report", Color.FromArgb(40, 167, 69)); btnExport.Dock = DockStyle.Fill; btnExport.Click += (s, e) => ExportApprenticeReport(app);
        actionGrid.Controls.Add(btnSick, 0, currentRow); actionGrid.Controls.Add(btnExport, 1, currentRow);

        Panel rightSidePanel = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(10, 0, 0, 0) };
        Label lblGoal = new Label() { Text = $"Goal: {app.CareerGoal}", Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.LightSkyBlue, Dock = DockStyle.Top, Height = 30 };
        Label lblNotes = new Label() { Text = "To-Do List / Day Plan:", Font = new Font("Segoe UI", 12, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
        TextBox txtNotes = new TextBox() { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Text = app.DailyPlan, ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 11) }; txtNotes.TextChanged += (s, e) => { app.DailyPlan = txtNotes.Text; Database.Save(); }; 
        rightSidePanel.Controls.Add(txtNotes); rightSidePanel.Controls.Add(lblNotes); rightSidePanel.Controls.Add(lblGoal);

        splitContainer.Controls.Add(actionGrid, 0, 0); splitContainer.Controls.Add(rightSidePanel, 1, 0); pnl.Controls.Add(splitContainer); pnl.Controls.Add(lblStats); return pnl;
    }

    /// <summary>
    /// Exportiert einen detaillierten Bericht über einen Lernenden in eine Textdatei.
    /// </summary>
    private void ExportApprenticeReport(Apprentice app) { string path = $"{app.FirstName}_{app.LastName}_Report.txt"; using (StreamWriter sw = new StreamWriter(path)) { sw.WriteLine($"=== APPRENTICE REPORT: {app.FirstName.ToUpper()} {app.LastName.ToUpper()} ==="); sw.WriteLine($"Company: {app.CompanyName} | Trainer: {app.TrainerName}\nDepartment: {app.Department} | Year: {app.CurrentYear}"); sw.WriteLine($"Goal: {app.CareerGoal}"); sw.WriteLine($"Sick Days: {app.SickDays} | Attendance: {app.AttendanceRate}% | Overall GPA: {app.OverallGPA:F2}\n\n--- TO-DO LIST / DAY PLAN ---"); sw.WriteLine(string.IsNullOrWhiteSpace(app.DailyPlan) ? "No plan recorded." : app.DailyPlan); sw.WriteLine("\n--- SUBJECTS & EXAMS ---"); foreach (var sub in app.Subjects) { sw.WriteLine($"> {sub.Name} (Avg: {sub.AverageGrade:F2})"); foreach (var t in sub.Tests) sw.WriteLine($"    - {t.TestName}: {t.Grade:F1}"); } sw.WriteLine("\n--- WORK JOURNALS ---"); foreach (var j in app.WorkJournals) sw.WriteLine($"[{j.Date:d}] {j.HoursWorked}h - {j.TaskDescription}"); } MessageBox.Show($"Report exported successfully to:\n{Path.GetFullPath(path)}"); }

    /// <summary>
    /// Erstellt das Panel zur Verwaltung der Fächer und Noten eines Lernenden.
    /// </summary>
    private Panel CreateSubjectsPanel(Apprentice app) {
        Panel pnl = new Panel() { Dock = DockStyle.Fill }; Panel toolbar = new Panel() { Dock = DockStyle.Top, Height = 70 };
        Button btnBack = AppUI.CreateFlatButton("⬅ Back", Color.FromArgb(80, 80, 80)); btnBack.Location = new Point(0, 15); btnBack.Click += (s, e) => NavigateBack();
        Label lblTitle = new Label() { Text = $"Subjects for {app.FirstName}", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(170, 18) };
        toolbar.Controls.Add(btnBack); toolbar.Controls.Add(lblTitle); pnl.Controls.Add(toolbar);

        FlowLayoutPanel actionToolbar = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 60 };
        Button btnAdd = AppUI.CreateFlatButton("➕ Add Subject", Color.FromArgb(0, 122, 204));
        btnAdd.Click += (s, e) => { string name = AppUI.PromptInput("Subject Name:", "New Subject"); if (!string.IsNullOrWhiteSpace(name)) { app.Subjects.Add(new Subject { Name = name }); Database.Save(); ReplaceCurrentView(CreateSubjectsPanel(app)); } };
        Button btnEdit = AppUI.CreateFlatButton("✏️ Edit Selected", Color.FromArgb(63, 63, 70));
        btnEdit.Click += (s, e) => { DataGridView gridRef = pnl.Controls.OfType<DataGridView>().FirstOrDefault(); if (gridRef != null && gridRef.SelectedRows.Count > 0 && gridRef.SelectedRows[0].DataBoundItem is Subject sub) { string newName = AppUI.PromptInput("Edit Subject Name:", "Edit Subject", sub.Name); if (!string.IsNullOrWhiteSpace(newName)) { sub.Name = newName; Database.Save(); ReplaceCurrentView(CreateSubjectsPanel(app)); } } };
        Button btnDelete = AppUI.CreateFlatButton("🗑️ Delete Selected", Color.FromArgb(180, 50, 50));
        btnDelete.Click += (s, e) => { DataGridView gridRef = pnl.Controls.OfType<DataGridView>().FirstOrDefault(); if (gridRef != null && gridRef.SelectedRows.Count > 0 && gridRef.SelectedRows[0].DataBoundItem is Subject sub) { app.Subjects.Remove(sub); Database.Save(); ReplaceCurrentView(CreateSubjectsPanel(app)); } };
        actionToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

        DataGridView grid = AppUI.CreateModernGrid(); grid.DataSource = app.Subjects.ToList();
        if(grid.Columns["Id"] != null) grid.Columns["Id"]!.Visible = false; if(grid.Columns["Tests"] != null) grid.Columns["Tests"]!.Visible = false; if(grid.Columns["AverageGrade"] != null) grid.Columns["AverageGrade"]!.HeaderText = "Current Average";
        grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].DataBoundItem is Subject sub) NavigateForward(CreateTestsPanel(sub, app)); };

        pnl.Controls.Add(grid); pnl.Controls.Add(actionToolbar); grid.BringToFront(); return pnl;
    }

    /// <summary>
    /// Erstellt das Panel zur Verwaltung der Prüfungen und Noten innerhalb eines Fachs.
    /// </summary>
    private Panel CreateTestsPanel(Subject subject, Apprentice app) {
        Panel pnl = new Panel() { Dock = DockStyle.Fill }; Panel toolbar = new Panel() { Dock = DockStyle.Top, Height = 70 };
        Button btnBack = AppUI.CreateFlatButton("⬅ Back", Color.FromArgb(80, 80, 80)); btnBack.Location = new Point(0, 15); btnBack.Click += (s, e) => NavigateBack();
        Label lblTitle = new Label() { Text = $"{subject.Name} Exams", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(170, 18) };
        toolbar.Controls.Add(btnBack); toolbar.Controls.Add(lblTitle); pnl.Controls.Add(toolbar);

        Label lblAvg = new Label() { Dock = DockStyle.Top, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Height = 40, Text = $"Average Grade: {subject.AverageGrade:F2}" };

        FlowLayoutPanel actionToolbar = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 60 };
        Button btnAdd = AppUI.CreateFlatButton("➕ Add Exam", Color.FromArgb(0, 122, 204));
        btnAdd.Click += (s, e) => { string testName = AppUI.PromptInput("Enter Exam Name:", "New Exam"); if (string.IsNullOrWhiteSpace(testName)) return; string gradeStr = AppUI.PromptInput("Swiss Grade (1.0 to 6.0):", "Grade").Replace(',', '.'); if (double.TryParse(gradeStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double grade) && grade >= 1.0 && grade <= 6.0) { subject.Tests.Add(new TestResult { TestName = testName, Grade = Math.Round(grade * 2, MidpointRounding.AwayFromZero) / 2.0 }); Database.Save(); ReplaceCurrentView(CreateTestsPanel(subject, app)); } else MessageBox.Show("Invalid Grade. Please use 1.0 to 6.0"); };
        Button btnEdit = AppUI.CreateFlatButton("✏️ Edit Selected", Color.FromArgb(63, 63, 70));
        btnEdit.Click += (s, e) => { DataGridView gridRef = pnl.Controls.OfType<DataGridView>().FirstOrDefault(); if (gridRef != null && gridRef.SelectedRows.Count > 0 && gridRef.SelectedRows[0].DataBoundItem is TestResult tr) { string testName = AppUI.PromptInput("Enter Exam Name:", "Edit Exam", tr.TestName); if (string.IsNullOrWhiteSpace(testName)) return; string gradeStr = AppUI.PromptInput("Swiss Grade (1.0 to 6.0):", "Edit Grade", tr.Grade.ToString()).Replace(',', '.'); if (double.TryParse(gradeStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double grade) && grade >= 1.0 && grade <= 6.0) { tr.TestName = testName; tr.Grade = Math.Round(grade * 2, MidpointRounding.AwayFromZero) / 2.0; Database.Save(); ReplaceCurrentView(CreateTestsPanel(subject, app)); } else MessageBox.Show("Invalid Grade."); } };
        Button btnDelete = AppUI.CreateFlatButton("🗑️ Delete Selected", Color.FromArgb(180, 50, 50));
        btnDelete.Click += (s, e) => { DataGridView gridRef = pnl.Controls.OfType<DataGridView>().FirstOrDefault(); if (gridRef != null && gridRef.SelectedRows.Count > 0 && gridRef.SelectedRows[0].DataBoundItem is TestResult tr) { subject.Tests.Remove(tr); Database.Save(); ReplaceCurrentView(CreateTestsPanel(subject, app)); } };
        actionToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

        DataGridView grid = AppUI.CreateModernGrid(); grid.DataSource = subject.Tests.ToList();
        if(grid.Columns["TestName"] != null) { grid.Columns["TestName"]!.HeaderText = "Exam / Test Name"; grid.Columns["TestName"]!.FillWeight = 200; } if(grid.Columns["Grade"] != null) { grid.Columns["Grade"]!.HeaderText = "Swiss Grade"; }
        pnl.Controls.Add(grid); pnl.Controls.Add(actionToolbar); pnl.Controls.Add(lblAvg); grid.BringToFront(); return pnl;
    }

    /// <summary>
    /// Erstellt das Panel zur Verwaltung der Arbeitstagebücher eines Lernenden.
    /// </summary>
    private Panel CreateJournalsPanel(Apprentice app) {
        Panel pnl = new Panel() { Dock = DockStyle.Fill }; Panel toolbar = new Panel() { Dock = DockStyle.Top, Height = 70 };
        Button btnBack = AppUI.CreateFlatButton("⬅ Back", Color.FromArgb(80, 80, 80)); btnBack.Location = new Point(0, 15); btnBack.Click += (s, e) => NavigateBack();
        Label lblTitle = new Label() { Text = $"Work Journals: {app.FirstName}", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(170, 18) };
        toolbar.Controls.Add(btnBack); toolbar.Controls.Add(lblTitle); pnl.Controls.Add(toolbar);

        FlowLayoutPanel actionToolbar = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 60 };
        Button btnAdd = AppUI.CreateFlatButton("➕ Add Journal", Color.FromArgb(0, 122, 204));
        btnAdd.Click += (s, e) => { JournalEntryDialog dialog = new JournalEntryDialog(); if (dialog.ShowDialog() == DialogResult.OK) { app.WorkJournals.Add(new WorkJournal { TaskDescription = dialog.TaskDesc, HoursWorked = dialog.Hours }); Database.Save(); ReplaceCurrentView(CreateJournalsPanel(app)); } };
        Button btnDelete = AppUI.CreateFlatButton("🗑️ Delete Selected", Color.FromArgb(180, 50, 50));
        btnDelete.Click += (s, e) => { DataGridView gridRef = pnl.Controls.OfType<DataGridView>().FirstOrDefault(); if (gridRef != null && gridRef.SelectedRows.Count > 0 && gridRef.SelectedRows[0].DataBoundItem is WorkJournal wj) { app.WorkJournals.Remove(wj); Database.Save(); ReplaceCurrentView(CreateJournalsPanel(app)); } };
        actionToolbar.Controls.AddRange(new Control[] { btnAdd, btnDelete });

        DataGridView grid = AppUI.CreateModernGrid(); grid.DataSource = app.WorkJournals.OrderByDescending(j => j.Date).ToList();
        if(grid.Columns["Id"] != null) grid.Columns["Id"]!.Visible = false; if(grid.Columns["Date"] != null) { grid.Columns["Date"]!.DefaultCellStyle.Format = "d"; grid.Columns["Date"]!.FillWeight = 80; } if(grid.Columns["HoursWorked"] != null) { grid.Columns["HoursWorked"]!.HeaderText = "Hours"; grid.Columns["HoursWorked"]!.FillWeight = 50; } if(grid.Columns["TaskDescription"] != null) { grid.Columns["TaskDescription"]!.HeaderText = "Task Description"; grid.Columns["TaskDescription"]!.FillWeight = 300; }
        grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].DataBoundItem is WorkJournal wj) { JournalEntryDialog dialog = new JournalEntryDialog(wj); if (dialog.ShowDialog() == DialogResult.OK) { wj.TaskDescription = dialog.TaskDesc; wj.HoursWorked = dialog.Hours; Database.Save(); ReplaceCurrentView(CreateJournalsPanel(app)); } } };

        pnl.Controls.Add(grid); pnl.Controls.Add(actionToolbar); grid.BringToFront(); return pnl;
    }

    /// <summary>
    /// Lädt die Daten für alle Verzeichnis-Grids neu.
    /// </summary>
    private void RefreshGrids() { if (_apprenticeGrid != null) { _apprenticeGrid.DataSource = null; _apprenticeGrid.DataSource = Database.Data.Apprentices.ToList(); HideCols(_apprenticeGrid, "Id", "WorkJournals", "Subjects", "FullName", "DailyPlan", "IsAtRisk", "PinCode", "AttendanceRate", "CareerGoal", "CasinoCredits"); } if (_companyGrid != null) { _companyGrid.DataSource = null; _companyGrid.DataSource = Database.Data.Companies.ToList(); HideCols(_companyGrid, "Id"); } if (_trainerGrid != null) { _trainerGrid.DataSource = null; _trainerGrid.DataSource = Database.Data.Trainers.ToList(); HideCols(_trainerGrid, "Id", "FullName", "PinCode", "CasinoCredits"); } }
    /// <summary>
    /// Versteckt die angegebenen Spalten in einem DataGridView.
    /// </summary>
    private void HideCols(DataGridView g, params string[] cols) { foreach(var c in cols) if(g.Columns[c] != null) g.Columns[c]!.Visible = false; }
    
    /// <summary>
    /// Öffnet den Bearbeitungsdialog für den ausgewählten Lernenden.
    /// </summary>
    private void EditApprentice() { if (_apprenticeGrid.SelectedRows.Count > 0 && _apprenticeGrid.SelectedRows[0].DataBoundItem is Apprentice a) { if (new EditApprenticeDialog(a).ShowDialog() == DialogResult.OK) { RefreshGrids(); } } }
    /// <summary>
    /// Fügt einen neuen Lernenden zur Datenbank hinzu.
    /// </summary>
    private void AddApprentice() { string f = AppUI.PromptInput("First Name:", "New"); if(!string.IsNullOrWhiteSpace(f)) { Database.Data.Apprentices.Add(new Apprentice { FirstName = f, LastName = AppUI.PromptInput("Last Name:", "") }); Database.Save(); RefreshGrids(); } }
    /// <summary>
    /// Löscht den ausgewählten Lernenden aus der Datenbank.
    /// </summary>
    private void DeleteApprentice() { if(_apprenticeGrid.SelectedRows.Count > 0 && _apprenticeGrid.SelectedRows[0].DataBoundItem is Apprentice a) { Database.Data.Apprentices.Remove(a); Database.Save(); RefreshGrids(); } }
    
    /// <summary>
    /// Öffnet den Bearbeitungsdialog für die ausgewählte Firma.
    /// </summary>
    private void EditCompany() { if (_companyGrid.SelectedRows.Count > 0 && _companyGrid.SelectedRows[0].DataBoundItem is Company c) { if (new EditCompanyDialog(c).ShowDialog() == DialogResult.OK) { RefreshGrids(); } } }
    /// <summary>
    /// Fügt eine neue Firma zur Datenbank hinzu.
    /// </summary>
    private void AddCompany() { string n = AppUI.PromptInput("Company Name:", "New"); if(!string.IsNullOrWhiteSpace(n)) { Database.Data.Companies.Add(new Company { Name = n, Location = AppUI.PromptInput("Location:", "") }); Database.Save(); RefreshGrids(); } }
    /// <summary>
    /// Löscht die ausgewählte Firma aus der Datenbank.
    /// </summary>
    private void DeleteCompany() { if(_companyGrid.SelectedRows.Count > 0 && _companyGrid.SelectedRows[0].DataBoundItem is Company c) { Database.Data.Companies.Remove(c); Database.Save(); RefreshGrids(); } }
    
    /// <summary>
    /// Öffnet den Bearbeitungsdialog für den ausgewählten Ausbilder.
    /// </summary>
    private void EditTrainer() { if (_trainerGrid.SelectedRows.Count > 0 && _trainerGrid.SelectedRows[0].DataBoundItem is VocationalTrainer t) { if (new EditTrainerDialog(t).ShowDialog() == DialogResult.OK) { RefreshGrids(); } } }
    /// <summary>
    /// Fügt einen neuen Ausbilder zur Datenbank hinzu.
    /// </summary>
    private void AddTrainer() { string n = AppUI.PromptInput("Trainer First Name:", "New"); if(!string.IsNullOrWhiteSpace(n)) { Database.Data.Trainers.Add(new VocationalTrainer { FirstName = n, LastName = AppUI.PromptInput("Trainer Last Name:", ""), Email = AppUI.PromptInput("Email:", "") }); Database.Save(); RefreshGrids(); } }
    /// <summary>
    /// Löscht den ausgewählten Ausbilder aus der Datenbank.
    /// </summary>
    private void DeleteTrainer() { if(_trainerGrid.SelectedRows.Count > 0 && _trainerGrid.SelectedRows[0].DataBoundItem is VocationalTrainer t) { Database.Data.Trainers.Remove(t); Database.Save(); RefreshGrids(); } }
}

public class EditCompanyDialog : Form
{
    /// <summary>
    /// Initialisiert ein Dialogfeld zum Bearbeiten der Details einer Firma.
    /// </summary>
    public EditCompanyDialog(Company c) { Text = "Edit Company"; Size = new Size(400, 300); StartPosition = FormStartPosition.CenterParent; BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false; Label lblName = new Label() { Text = "Company Name:", Left = 20, Top = 20, AutoSize = true }; TextBox txtName = new TextBox() { Left = 20, Top = 40, Width = 340, Text = c.Name, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblLoc = new Label() { Text = "Location:", Left = 20, Top = 80, AutoSize = true }; TextBox txtLoc = new TextBox() { Left = 20, Top = 100, Width = 340, Text = c.Location, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Button btnSave = AppUI.CreateFlatButton("Save Changes", Color.FromArgb(0, 122, 204)); btnSave.Location = new Point(120, 160); btnSave.Click += (s, e) => { c.Name = txtName.Text; c.Location = txtLoc.Text; Database.Save(); DialogResult = DialogResult.OK; }; Controls.Add(lblName); Controls.Add(txtName); Controls.Add(lblLoc); Controls.Add(txtLoc); Controls.Add(btnSave); }
}

public class EditTrainerDialog : Form
{
    /// <summary>
    /// Initialisiert ein Dialogfeld zum Bearbeiten der Details eines Ausbilders.
    /// </summary>
    public EditTrainerDialog(VocationalTrainer t) { Text = "Edit Trainer Profile"; Size = new Size(400, 480); StartPosition = FormStartPosition.CenterParent; BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false; Label lblFirst = new Label() { Text = "First Name:", Left = 20, Top = 20, AutoSize = true }; TextBox txtFirst = new TextBox() { Left = 20, Top = 40, Width = 340, Text = t.FirstName, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblLast = new Label() { Text = "Last Name:", Left = 20, Top = 80, AutoSize = true }; TextBox txtLast = new TextBox() { Left = 20, Top = 100, Width = 340, Text = t.LastName, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblEmail = new Label() { Text = "Email:", Left = 20, Top = 140, AutoSize = true }; TextBox txtEmail = new TextBox() { Left = 20, Top = 160, Width = 340, Text = t.Email, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblPin = new Label() { Text = "Login PIN:", Left = 20, Top = 200, AutoSize = true }; TextBox txtPin = new TextBox() { Left = 20, Top = 220, Width = 340, Text = t.PinCode, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblCreds = new Label() { Text = "Casino Credits:", Left = 20, Top = 260, AutoSize = true }; TextBox txtCreds = new TextBox() { Left = 20, Top = 280, Width = 340, Text = t.CasinoCredits.ToString(), BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Button btnSave = AppUI.CreateFlatButton("Save Changes", Color.FromArgb(0, 122, 204)); btnSave.Location = new Point(120, 360); btnSave.Click += (s, e) => { t.FirstName = txtFirst.Text; t.LastName = txtLast.Text; t.Email = txtEmail.Text; t.PinCode = txtPin.Text; if(int.TryParse(txtCreds.Text, out int c)) t.CasinoCredits = c; Database.Save(); DialogResult = DialogResult.OK; }; Controls.Add(lblFirst); Controls.Add(txtFirst); Controls.Add(lblLast); Controls.Add(txtLast); Controls.Add(lblEmail); Controls.Add(txtEmail); Controls.Add(lblPin); Controls.Add(txtPin); Controls.Add(lblCreds); Controls.Add(txtCreds); Controls.Add(btnSave); }
}

public class EditApprenticeDialog : Form
{
    /// <summary>
    /// Initialisiert ein Dialogfeld zum Bearbeiten der Details eines Lernenden.
    /// </summary>
    public EditApprenticeDialog(Apprentice app) { Text = "Edit Apprentice Profile"; Size = new Size(400, 680); StartPosition = FormStartPosition.CenterParent; BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false; Label lblFirst = new Label() { Text = "First Name:", Left = 20, Top = 10, AutoSize = true }; TextBox txtFirst = new TextBox() { Left = 20, Top = 30, Width = 340, Text = app.FirstName, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblLast = new Label() { Text = "Last Name:", Left = 20, Top = 70, AutoSize = true }; TextBox txtLast = new TextBox() { Left = 20, Top = 90, Width = 340, Text = app.LastName, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblDept = new Label() { Text = "Department:", Left = 20, Top = 130, AutoSize = true }; TextBox txtDept = new TextBox() { Left = 20, Top = 150, Width = 340, Text = app.Department, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblGoal = new Label() { Text = "Career Goal:", Left = 20, Top = 190, AutoSize = true }; TextBox txtGoal = new TextBox() { Left = 20, Top = 210, Width = 340, Text = app.CareerGoal, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblCompany = new Label() { Text = "Assign Company:", Left = 20, Top = 250, AutoSize = true }; ComboBox cbCompany = new ComboBox() { Left = 20, Top = 270, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; cbCompany.Items.Add("Not Assigned"); cbCompany.Items.AddRange(Database.Data.Companies.Select(c => c.Name).ToArray()); cbCompany.SelectedItem = Database.Data.Companies.Any(c => c.Name == app.CompanyName) ? app.CompanyName : "Not Assigned"; Label lblTrainer = new Label() { Text = "Assign Trainer:", Left = 20, Top = 310, AutoSize = true }; ComboBox cbTrainer = new ComboBox() { Left = 20, Top = 330, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; cbTrainer.Items.Add("Not Assigned"); cbTrainer.Items.AddRange(Database.Data.Trainers.Select(t => t.FullName).ToArray()); cbTrainer.SelectedItem = Database.Data.Trainers.Any(t => t.FullName == app.TrainerName) ? app.TrainerName : "Not Assigned"; Label lblPin = new Label() { Text = "Account Login PIN:", Left = 20, Top = 370, AutoSize = true }; TextBox txtPin = new TextBox() { Left = 20, Top = 390, Width = 340, Text = app.PinCode, BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Label lblCreds = new Label() { Text = "Casino Credits:", Left = 20, Top = 430, AutoSize = true }; TextBox txtCreds = new TextBox() { Left = 20, Top = 450, Width = 340, Text = app.CasinoCredits.ToString(), BackColor = Color.FromArgb(43,43,43), ForeColor = Color.White }; Button btnSave = AppUI.CreateFlatButton("Save Changes", Color.FromArgb(0, 122, 204)); btnSave.Location = new Point(120, 520); btnSave.Click += (s, e) => { app.FirstName = txtFirst.Text; app.LastName = txtLast.Text; app.Department = txtDept.Text; app.CareerGoal = txtGoal.Text; app.CompanyName = cbCompany.SelectedItem?.ToString() ?? "Not Assigned"; app.TrainerName = cbTrainer.SelectedItem?.ToString() ?? "Not Assigned"; app.PinCode = txtPin.Text; if(int.TryParse(txtCreds.Text, out int c)) app.CasinoCredits = c; Database.Save(); DialogResult = DialogResult.OK; }; Controls.Add(lblFirst); Controls.Add(txtFirst); Controls.Add(lblLast); Controls.Add(txtLast); Controls.Add(lblDept); Controls.Add(txtDept); Controls.Add(lblGoal); Controls.Add(txtGoal); Controls.Add(lblCompany); Controls.Add(cbCompany); Controls.Add(lblTrainer); Controls.Add(cbTrainer); Controls.Add(lblPin); Controls.Add(txtPin); Controls.Add(lblCreds); Controls.Add(txtCreds); Controls.Add(btnSave); }
}