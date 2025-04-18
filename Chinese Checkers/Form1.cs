using System.Drawing.Drawing2D;

namespace Chinese_Checkers
{
    public partial class Form1 : Form
    {
        private List<Node> nodes = new List<Node>();
        private List<Piece> pieces = new List<Piece>();
        private const int CircleRadius = 20;
        private const int PieceRadius = 15;
        private const int Distance = 30;
        private Piece selectedPiece = null;
        private int numberOfPlayers;

        public Form1()
        {
            InitializeComponent();
           
            this.DoubleBuffered = true;
            this.ClientSize = new Size(1200, 1200);
            this.Paint += Form1_Paint;
            this.Resize += (s, e) => { GenerateNodes(); Invalidate(); };
            this.MouseClick += Form1_MouseClick;

            GenerateNodes();
            SetupPieces();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            using (Font font = new Font("Segoe UI", 8))
            using (Brush brush = Brushes.Black)
            {
                PointF center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

                foreach (var node in nodes)
                {
                    PointF pos = node.GetPixelPosition(center, Distance);

                    DrawCircle(g, pos, Brushes.White);
                    g.DrawEllipse(Pens.Black,
                        pos.X - CircleRadius,
                        pos.Y - CircleRadius,
                        CircleRadius * 2,
                        CircleRadius * 2);

                   
                    
                }
                RenderPieces(g, center);

            }
            

        }
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            PointF center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

            // Try selecting a piece
            foreach (var piece in pieces)
            {
                PointF pos = piece.GetPixelPosition(center, Distance);
                float dx = e.X - pos.X;
                float dy = e.Y - pos.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance <= PieceRadius)
                {
                    selectedPiece = piece;
                    Invalidate();
                    return;
                }
            }

            // If a piece is already selected, try to move it
            if (selectedPiece != null)
            {
                foreach (var node in nodes)
                {
                    PointF pos = node.GetPixelPosition(center, Distance);
                    float dx = e.X - pos.X;
                    float dy = e.Y - pos.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance <= CircleRadius && !node.isOccupied)
                    {
                        // Move piece
                        selectedPiece.CurrentNode.isOccupied = false;
                        selectedPiece.CurrentNode = node;
                        node.isOccupied = true;
                        selectedPiece = null;
                        Invalidate();
                        return;
                    }
                }
            }

            // Clicked on nothing → deselect
            selectedPiece = null;
            Invalidate();
        }

        private void DrawCircle(Graphics g, PointF center, Brush brush)
        {
            g.FillEllipse(brush,
                center.X - CircleRadius,
                center.Y - CircleRadius,
                CircleRadius * 2,
                CircleRadius * 2);
        }

        private void DrawPiece(Graphics g, PointF center, Brush brush)
        {
            g.FillEllipse(brush,
                center.X - PieceRadius,
                center.Y - PieceRadius,
                PieceRadius * 2,
                PieceRadius * 2);
        }
        private void SetupPieces()
        {
            pieces.Clear();
                var startNode = nodes.First(n => n.Q == -4 && n.R == 3 && n.S == 1);
            pieces.Add(new Piece(startNode, 1));
        }

        public void RenderPieces(Graphics g, PointF origin)
        {

            if(numberOfPlayers == 2)
            {

                foreach (var piece in pieces)
                {
                    for (int player = 1; player <= numberOfPlayers; player++)
                    {  
                        PointF pos = piece.GetPixelPosition(origin, Distance);
                        if( player == 1 )
                        {
                           
                        }
                        
                    }
                }
                    
                    

                }

            }
            
        


        private void GenerateNodes()
        {
            nodes.Clear();
            PointF center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

            HashSet<(int q, int r, int s)> coords = new();

            Point[] HexDirections = new Point[]
            {
                new Point(1, 0),    // Right
                new Point(1, -1),   // Top-right
                new Point(0, -1),   // Top-left
                new Point(-1, 0),   // Left
                new Point(-1, 1),   // Bottom-left
                new Point(0, 1)     // Bottom-right
            };

            for (int x = -8; x <= 8; x++)
            {
                for (int y = -8; y <= 8; y++)
                {
                    for (int z = -8; z <= 8; z++)
                    {
                        if (((Math.Abs(x) <= 4) && (Math.Abs(y) <= 4)) ||
                            ((Math.Abs(y) <= 4) && (Math.Abs(z) <= 4)) ||
                            ((Math.Abs(z) <= 4) && (Math.Abs(x) <= 4)))
                        {
                            if (x + y + z == 0)
                            {
                                coords.Add((x, y, z));
                            }
                        }
                    }
                }
            }

            foreach (var (q, r, s) in coords)
            {
                nodes.Add(new Node(q, r, s));
            }
        }

        public void TrackPieces()
        {
            
        }

        public void RenderPieces(Graphics g, PointF origin, Brush brush)
        {
            foreach (var piece in pieces)
            {
                PointF pos = piece.GetPixelPosition(origin, Distance);
                DrawPiece(g, pos, brush);
            }
        }


        private PointF CubeToPixel(int q, int r, int s, PointF origin)
        {
            float x = Distance * (float)(Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r);
            float y = Distance * (float)(3.0 / 2 * r);
            return new PointF(origin.X + x, origin.Y + y);
        }

        class Node
        {
            public int Q, R, S;
            public List<Node> Neighbors = new List<Node>();
            public bool isOccupied = false;
            public int Player = 0; // 0 = unowned

            public Node(int q, int r, int s)
            {
                Q = q;
                R = r;
                S = s;
            }

            public PointF GetPixelPosition(PointF origin, float distance)
            {
                float x = distance * (float)(Math.Sqrt(3) * Q + Math.Sqrt(3) / 2 * R);
                float y = distance * (float)(3.0 / 2 * R);
                return new PointF(origin.X + x, origin.Y + y);
            }
        }

        class Piece
        {
            public Node CurrentNode;
            public int Player;

            public Piece(Node node, int player)
            {
                CurrentNode = node;
                Player = player;
            }

            public PointF GetPixelPosition(PointF origin, float distance)
            {
                return CurrentNode.GetPixelPosition(origin, distance);
            }
        }

        private int PromptPlayerCount()
        {
            Form prompt = new Form()
            {
                Width = 250,
                Height = 350,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Players",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label lblText = new Label() { Left = 20, Top = 20, Text = "Number of players:" };
            NumericUpDown numPlayers = new NumericUpDown()
            {
                Left = 20,
                Top = 50,
                Width = 180,
                Minimum = 2,
                Maximum = 6,
                Value = 2
            };
            Button confirm = new Button() { Text = "Start", Left = 130, Width = 70, Top = 90, DialogResult = DialogResult.OK };

            confirm.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(lblText);
            prompt.Controls.Add(numPlayers);
            prompt.Controls.Add(confirm);
            prompt.AcceptButton = confirm;

            return prompt.ShowDialog() == DialogResult.OK ? (int)numPlayers.Value : 2; // Default 2 if canceled
        }




    }




}
