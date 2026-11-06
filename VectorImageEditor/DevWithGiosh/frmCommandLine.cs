using EditorModel.Figures;
using EditorModel.Figures.Basket;
using EditorModel.Figures.Extentions;
using EditorModel.Selections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorImageEditor.Controls;

namespace VectorImageEditor.DevWithGiosh
{
    public partial class frmCommandLine : Form
    {
        #region DICHIARAZIONI
        #region PARAMETRI
        private MacroActions MAs;
        #endregion PARAMETRI
        private List<(int X, int Y)> punti = new List<(int X, int Y)>();

        #endregion DICHIARAZIONI
        public frmCommandLine(MacroActions pMacroActions)
        {
            MAs = pMacroActions;
            InitializeComponent();

            dataGridView1.Columns.Clear();
            dataGridView1.AllowUserToAddRows = true; // lascia la riga vuota per aggiungere
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            // Colonna X
            var colX = new DataGridViewTextBoxColumn
            {
                Name = "colX",
                HeaderText = "X",
                ValueType = typeof(double)
            };
            // Colonna Y
            var colY = new DataGridViewTextBoxColumn
            {
                Name = "colY",
                HeaderText = "Y",
                ValueType = typeof(double)
            };

            dataGridView1.Columns.Add(colX);
            dataGridView1.Columns.Add(colY);

            // Aggancia l'evento per validare input numerico
            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
        }

        #region COMANDI_2_COORDINATE

        #region FILTRA_COORDINATE
        private void txtX0_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Blocca il carattere
            }
        }

        private void txtY0_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Blocca il carattere
            }
        }

        private void txtX1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Blocca il carattere
            }
        }

        private void txtY1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Blocca il carattere
            }
        }
        #endregion FILTRA_COORDINATE

        #region MOTORE
        private void CreaFigura2Coordinate(MacroGeometrySelectionMode pGeometry)
        {
            if (string.IsNullOrEmpty(txtX1.Text) || string.IsNullOrEmpty(txtY1.Text) || string.IsNullOrEmpty(txtX0.Text) || string.IsNullOrEmpty(txtY0.Text))
                return;

            int x0 = Convert.ToInt32(txtX0.Text);
            int y0 = Convert.ToInt32(txtY0.Text);
            int x1 = Convert.ToInt32(txtX1.Text);
            int y1 = Convert.ToInt32(txtY1.Text);

            CreaFigura2CoordinateRelaitve(pGeometry, x0, y0, x1, y1);
        }

        private void CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode pGeometry, int x0, int y0, int x1, int y1)
        {

            //seleziona il tool prescelto
            MAs.GeometryModeSelected = pGeometry;

            //crea la figura e il cursore
            MAs.CreateFigure(MAs.GeometryModeSelected);

            // Le coordinate del comando sono coordinate logiche del disegno.
            // Gli eventi simulati passano dal controller canvas, che divide le
            // coordinate ricevute per lo zoom: compensiamo qui affinché le
            // misure generate restino uguali al 100% e il canvas le ingrandisca
            // poi normalmente secondo lo zoom corrente.
            float scaleFactor = Math.Max(0.01f, MAs._selectionController.ScaleFactor);
            int screenX0 = (int)Math.Round(x0 * scaleFactor);
            int screenY0 = (int)Math.Round(y0 * scaleFactor);
            int screenX1 = (int)Math.Round(x1 * scaleFactor);
            int screenY1 = (int)Math.Round(y1 * scaleFactor);

            //prepara
            MouseEventArgs e0 = new MouseEventArgs(MouseButtons.Left, 1, screenX0, screenY0, 0);
            MouseEventArgs e1 = new MouseEventArgs(MouseButtons.Left, 1, screenX1, screenY1, 0);
            Keys ModifierKeys = new Keys();

            //disegna
            MAs.Canvas_MouseDown(e0, ModifierKeys);
            MAs.Canvas_MouseMove(e1, ModifierKeys);
            MAs.Canvas_MouseUp(e1, ModifierKeys);
            MAs.GeometryModeSelected = MacroGeometrySelectionMode.None;
        }
        #endregion MOTORE

       


        private void brnCampo_Click(object sender, EventArgs e)
        {
            CourtyCampionato camp = CourtyCampionato.NONE;
            if (radioFIBA.Checked)
                camp = CourtyCampionato.FIBA;
            else if (radioNBA.Checked)
                camp = CourtyCampionato.NBA;
            else if (radioWNBA.Checked)
                camp = CourtyCampionato.WNBA;
            else if (radioNCAA_M.Checked)
                camp = CourtyCampionato.NCAA_M;
            else if (radioNCAA_W.Checked)
                camp = CourtyCampionato.NCAA_W;

            bool isFull = false;
            bool IsOffense = true;
            int x = 0;
            int y = 0;

            if (radioOffense.Checked || radioFULL.Checked)
                IsOffense = true;
            else
                IsOffense = false;

            if (radioFULL.Checked)
                isFull = true;
            else
                isFull = false;

           

            if (!radioSenza.Checked)
            {
                x = 80;
                y = 80;
            }
            CreaBasketCourt(x, y, camp, IsFull: isFull, IsOffense: IsOffense);

        }



        private void CreaBasketCourt(int delta_x, int delta_y, CourtyCampionato camp, bool IsFull, bool IsOffense)
        {
            //tipo di campionato: FIBA, NBA, WNBA, NCAA (M), NCAA (W)
            MAs.Campionato = camp;

            //tipo oggett del campo di basket da disegnare 
            MAs.OggettoCampoBasket = BasketCourtObjectType.None;


            //prepara le specifiche del campo
            SpecificheCampo spec = default;
            if (camp == CourtyCampionato.FIBA)
                spec = BasketSpecifiche.FIBA;
            else if (camp == CourtyCampionato.NBA)
                spec = BasketSpecifiche.NBA;
            else if (camp == CourtyCampionato.WNBA)
                spec = BasketSpecifiche.WNBA;
            else if (camp == CourtyCampionato.NCAA_M)
                spec = BasketSpecifiche.NCAA_M;
            else if (camp == CourtyCampionato.NCAA_W)
                spec = BasketSpecifiche.NCAA_W;

            int FullWidth = spec.FullWidth;
            int FullHeight = spec.FullLength;

            //stabilisce se deve disegnare le linee fuori
            bool conBordo = (delta_x == 0 && delta_y == 0) ? false : true;            
            if (!IsOffense)
            {
                delta_y = 0;
            }
            if (conBordo) MAs.DrawPalazzetto = true;
           

            Point p0 = new Point(0, 0);
            Point p1 = new Point(0, 0);
            int incrementoCerchio = 0;
            //
            int x0 = delta_x;
            int y0 = delta_y;
            int x1 = 0;
            int y1 = 0;

            //deve disegnare palazzettto
            if (conBordo)
            {
                x0 = 0;
                y0 = 0;

                if (IsFull)
                {
                    //disegna meta campo
                    x1 = spec.FullWidth + 2 * delta_x;
                    y1 = spec.FullLength + 2 * delta_y;
                    CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, x0, y0, x1, y1);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.Palazzetto);
                }
                else
                {
                   if (IsOffense)
                   {
                        //disegna meta campo
                        x1 = spec.FullWidth + 2 * delta_x;
                        y1 = spec.MetaLengh + delta_y;
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, x0, y0, x1, y1);
                        MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.Palazzetto);

                   }
                   else
                   {
                        //disegna meta campo
                        x1 = spec.FullWidth + 2 *delta_x;
                        y1 = spec.MetaLengh + delta_y;
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, x0, y0, x1, y1);
                        MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.Palazzetto);
                    }
                }
            }



            //----------------------------------------------------------
            //PRIMO CAMPO OFFENSIVO o SINGOLO CAMPO OFFENSIVO/DIFERNSIVO
            //----------------------------------------------------------

            //disegna meta campo
            x0 = delta_x;
            y0 = delta_y;
            x1 = spec.FullWidth + delta_x;
            y1 = spec.MetaLengh  + delta_y;
            CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, x0, y0, x1, y1);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.CanestroAttacco : BasketSpecifiche.CampoDifesa);
            

            //tre punti
            p0 = IsOffense ? new Point(spec.x0_3pts + delta_x, y0 ) : new Point(spec.x0_3pts + delta_x, y1 - spec.MetaLengh );
            p1 = IsOffense ? new Point(spec.MetaLengh + delta_x, spec.MetaLengh + delta_y) : new Point(spec.MetaLengh + delta_x, y1 );
            MAs.OggettoCampoBasket = BasketCourtObjectType.LineaTiroTrePunti;
            CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.TrePuntiAttacco : BasketSpecifiche.TrePuntiDifesa);

            //area
            p0 = IsOffense ? new Point (spec.Areax0 + delta_x, y0 ) : new Point(spec.Areax0 + delta_x, y1 - spec.Areay1 );
            p1 = IsOffense ? new Point (spec.Areax1 + delta_x, spec.Areay1 + delta_y) : new Point(spec.Areax1 + delta_x, y1 );   
            CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.AreaAttacco : BasketSpecifiche.AreaDifesa);

            //lunetta
            incrementoCerchio = spec.CerchioCentroRaggio * 2;

            p0 = IsOffense ? new Point(spec.x0_Lunetta + delta_x, spec.y0_Lunetta + delta_y) : p0 = new Point(spec.x0_Lunetta + delta_x, y1 - spec.y0_Lunetta );
            p1 = IsOffense ? new Point(p0.X + incrementoCerchio , p0.Y + incrementoCerchio ) : new Point(p0.X + incrementoCerchio , p0.Y - incrementoCerchio ); 
            CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.SemicerchioSotto : MacroGeometrySelectionMode.SemicerchioSopra, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.CerchioTiroLiberoFuoriAttacco : BasketSpecifiche.CerchioTiroLiberoFuoriDifesa);
            CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.SemicerchioSopra : MacroGeometrySelectionMode.SemicerchioSotto, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.CerchioTiroLiberoDentroAttacco : BasketSpecifiche.CerchioTiroLiberoDentroDifesa);

            //oggetti campo basket
            p0 = new Point(x0 , y0 );
            p1 = new Point(x1 , y1 );
            MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.TaccheAttacco : BasketCourtObjectType.TaccheDifesa;
            CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.ReboundTaccheAttacco : BasketSpecifiche.ReboundTaccheDifesa);
            if (camp == CourtyCampionato.NBA || camp == CourtyCampionato.WNBA)
            {
                MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.LineaRimesseInterneSoloAttacco : BasketCourtObjectType.LineaRimesseInterneSoloDifesa;
                CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle((BasketSpecifiche.LineaRimessa));

            }
            if (camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W)
            {
                MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.LineaRimesseInterneSoloAttacco : BasketCourtObjectType.LineaRimesseInterneSoloDifesa;
                CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle((BasketSpecifiche.LineaRimessa));

            }
            //
            if ( camp == CourtyCampionato.NBA || camp == CourtyCampionato.WNBA || camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W || camp == CourtyCampionato.FIBA)
            {
                MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.LineaNoSfondoAttacco : BasketCourtObjectType.LineaNoSfondoDifesa;
                CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.LineaSfondoAttacco : BasketSpecifiche.LineaSfondoDifesa);

            }
            //
            MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.CanestroSoloAttacco : BasketCourtObjectType.CanestroSoloDifesa;
            CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.CanestroAttacco : BasketSpecifiche.CanestroDifesa);
            //
            if (camp == CourtyCampionato.WNBA)
            {
                MAs.OggettoCampoBasket = BasketCourtObjectType.HasMarks;
                CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.Marks);
            }

            //cerchio di metà campo
            p0 = IsOffense ?  new Point(spec.CerchioCentro_x0 + delta_x, spec.CerchioCentro_y0 + delta_y) : new Point(spec.CerchioCentro_x0 + delta_x, delta_y);
            p1 = IsOffense ?  new Point(p0.X + incrementoCerchio , p0.Y + incrementoCerchio ) : new Point(p0.X + incrementoCerchio , p0.Y + incrementoCerchio );
            CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.SemicerchioSopra : MacroGeometrySelectionMode.SemicerchioSotto, p0.X, p0.Y, p1.X, p1.Y);
            MAs.SetSelectionContainerGeometryTitle(IsOffense ? BasketSpecifiche.CerchioMetaCampoAttacco : BasketSpecifiche.CerchioMetaCampoDifesa);

            if (conBordo && !IsFull)
            {
      
                //LINEE PANCHINE (SOLO ESTERNE)
                p0 = new Point(0, 0);
                p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.LineaPanchineAttacco : BasketCourtObjectType.LineaPanchineDifesa;
                CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaPanchine);
                
                if (camp == CourtyCampionato.FIBA)
                {
                    p0 = new Point(0, 0);
                    p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                    MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.LineaRimesseEsterneAttacco : BasketCourtObjectType.LineaRimesseEsterneDifessa;
                    CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaRimessa);

                }
                
                if(camp == CourtyCampionato.NBA || camp == CourtyCampionato.WNBA)
                {
                    p0 = new Point(0, 0);
                    p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                    MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.ZonaCambiSoloAttacco : BasketCourtObjectType.ZonaCambiSoloDifesa;
                    CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.ZonaCambi);
                }

                if (camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W)
                {
                    p0 = new Point(0, 0);
                    p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                    MAs.OggettoCampoBasket = IsOffense ? BasketCourtObjectType.LineaRimesseEsterneAttacco : BasketCourtObjectType.LineaRimesseEsterneDifessa;
                    CreaFigura2CoordinateRelaitve(IsOffense ? MacroGeometrySelectionMode.OggettoCampoBasketAttacco : MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.ZonaCambi);
                }


            }


            //--------------------------
            //SECONDO CAMPO (DIFENSIVO)
            //--------------------------


            //eventuale disegna la secxonda parte
            if (IsFull)
            {
                //Se serve indica che è
                MAs.DrawFullCamp = true;

                x1 = spec.FullWidth  + delta_x;
                y1 = spec.MetaLengh  + delta_y;
                //int y1full = 2 * spec.MetaLengh - 1 + delta_y;
                int y1full = 2 * spec.MetaLengh  + delta_y;
                int delta_yy = spec.MetaLengh;
                int y1startfull = 0;

                //campo difesa
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, x0, y0 + delta_yy, x1, y1 + delta_yy);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.CampoDifesa);

                x0 = delta_x;
                y0 = delta_y;
                x1 = spec.FullWidth + delta_x;
                y1 = spec.MetaLengh + delta_y + delta_yy;

                //tre punti
                p0 = new Point(spec.x0_3pts + delta_x, y1 - spec.x1_3pts );
                p1 = new Point(spec.x1_3pts + delta_x, y1 );
                MAs.OggettoCampoBasket = BasketCourtObjectType.LineaTiroTrePunti;
                CreaFigura2CoordinateRelaitve( MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.TrePuntiDifesa);


                //area
                p0 = new Point(spec.Areax0 + delta_x, y1 - spec.Areay1 );
                p1 = new Point(spec.Areax1 + delta_x, y1 );
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.Rectangle, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.AreaDifesa);
                
                //lunetta
                p0 = new Point(spec.x0_Lunetta + delta_x, y1 - spec.y0_Lunetta );
                p1 = new Point(p0.X + incrementoCerchio , p0.Y - incrementoCerchio);
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.SemicerchioSotto, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.CerchioTiroLiberoDentroDifesa);
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.SemicerchioSopra, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.CerchioTiroLiberoFuoriDifesa );


                //figure campo basket
                //p0 = new Point(0, 0);
                //p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);

                int yInizioTaccheDifesaFull = y1startfull + delta_yy;
                if (MAs.DrawPalazzetto)
                    yInizioTaccheDifesaFull += delta_y;
                p0 = new Point(x0, yInizioTaccheDifesaFull);
                p1 = new Point(x1 , y1full );
                MAs.OggettoCampoBasket = MAs.DrawPalazzetto ? BasketCourtObjectType.TaccheDifesaFullPalazzetto: BasketCourtObjectType.TaccheDifesaFull;
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.ReboundTaccheDifesa);
                if (camp == CourtyCampionato.NBA || camp == CourtyCampionato.WNBA)
                {
                    MAs.OggettoCampoBasket = BasketCourtObjectType.LineaRimesseInterneFull;
                    CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaRimessa);

                }

                p0 = new Point(x0, y1startfull + delta_yy);
                p1 = new Point(x1, y1full); 
                if (!conBordo && (camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W))
                {
                    MAs.OggettoCampoBasket = BasketCourtObjectType.LineaRimesseInterneFull;
                    CreaFigura2CoordinateRelaitve( MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaRimessa);

                }

                //linea no sfondo full
                yInizioTaccheDifesaFull = y1startfull + delta_yy;
                if (MAs.DrawPalazzetto)
                    yInizioTaccheDifesaFull += delta_y;
                p0 = new Point(x0, yInizioTaccheDifesaFull);
                p1 = new Point(x1, y1full);
                MAs.OggettoCampoBasket = MAs.DrawPalazzetto ? BasketCourtObjectType.LineaNoSfondoDifesaFullPalazzetto: BasketCourtObjectType.LineaNoSfondoDifesaFull ;
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaSfondoDifesa);
                
                //canestro difensifo full
                yInizioTaccheDifesaFull = y1startfull + delta_yy;
                if (MAs.DrawPalazzetto)
                    yInizioTaccheDifesaFull += delta_y;
                p0 = new Point(x0, yInizioTaccheDifesaFull);
                p1 = new Point(x1, y1full);
                MAs.OggettoCampoBasket = MAs.DrawPalazzetto ? BasketCourtObjectType.CanestroFullDifesaPalazzetto : BasketCourtObjectType.CanestroFullDifesa;
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.CanestroDifesa );
                
                //HasMark
                if (camp == CourtyCampionato.WNBA)
                {
                    MAs.OggettoCampoBasket = BasketCourtObjectType.HasMarks;
                    CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketDifesa, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.Marks);
                }

                //cerchio di metà campo
                p0 = new Point(spec.CerchioCentro_x0 + delta_x,  delta_y + delta_yy);
                p1 = new Point(p0.X + incrementoCerchio , p0.Y + incrementoCerchio );
                CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.SemicerchioSotto, p0.X, p0.Y, p1.X, p1.Y);
                MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.CerchioMetaCampoDifesa);

                if (conBordo )
                {
                    //LINEE PANCHINE (ESTERNE)
                    p0 = new Point(0, 0 );
                    p1 = new Point(2 * delta_x + FullWidth,2 * delta_y + FullHeight);
                    MAs.OggettoCampoBasket = BasketCourtObjectType.LineaPanchineFull;
                    CreaFigura2CoordinateRelaitve( MacroGeometrySelectionMode.OggettoCampoBasketAttacco, p0.X, p0.Y, p1.X, p1.Y);
                    MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaPanchine);

                    //LINEE RIMESSE
                    if (camp == CourtyCampionato.FIBA)
                    {
                        //FIBA: sono ESTTERNE
                        p0 = new Point(0, 0);
                        p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                        MAs.OggettoCampoBasket = BasketCourtObjectType.LineaRimesseEsterneFull;
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketAttacco, p0.X, p0.Y, p1.X, p1.Y);
                        MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaRimessa);
                    }
                    if  (camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W)
                    {
                        p0 = new Point(0, 0);
                        p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                        MAs.OggettoCampoBasket = BasketCourtObjectType.LineaRimesseInterneFullPalazzetto;
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketAttacco, p0.X, p0.Y, p1.X, p1.Y);
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketAttacco, p0.X, p0.Y, p1.X, p1.Y);
                        MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaRimessa);

                    }

                    
                    if (camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W)
                    {
                        p0 = new Point(0, 0);
                        p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                        MAs.OggettoCampoBasket = BasketCourtObjectType.LineaRimesseEsterneFullPalazzetto;
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketAttacco, p0.X, p0.Y, p1.X, p1.Y);
                        MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.LineaRimessa);

                    }
                    
                    //ZONA CAMBI
                    if (camp == CourtyCampionato.NBA || camp == CourtyCampionato.WNBA)
                    {
                        p0 = new Point(0, 0);
                        p1 = new Point(2 * delta_x + FullWidth, 2 * delta_y + FullHeight);
                        MAs.OggettoCampoBasket = BasketCourtObjectType.ZonaCambiFull;
                        CreaFigura2CoordinateRelaitve(MacroGeometrySelectionMode.OggettoCampoBasketAttacco, p0.X, p0.Y, p1.X, p1.Y);
                        MAs.SetSelectionContainerGeometryTitle(BasketSpecifiche.ZonaCambi);
                    }

                }

            }

            MAs.Campionato = CourtyCampionato.NONE;
            MAs.OggettoCampoBasket = BasketCourtObjectType.None;
            MAs.DrawFullCamp = false;
            MAs.DrawPalazzetto = false;
        }


        private void btnSvg_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Svg);
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Rectangle);
        }
        private void btnCircle_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Circle);
        }
        private void bbtnArc_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Arc);
        }
        private void btnCord_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Chord);
        }
        private void btnPie_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Pie);
        }

        private void btnRound_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Rounded);
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Ellipse);
        }

        private void btnRomb_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Romb);
        }

        private void btnRegular3_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular3);
        }

        private void btnRegular4_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular4);
        }

        private void btnRegular5_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular5);
        }

        private void btnRegular6_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular6);
        }

        private void btnRegular7_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular7);
        }

        private void btnRegular8_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular8);
        }

        private void btnRegular9_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular9);
        }

        private void btnRegular10_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Regular10);
        }
        private void btnSquare_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.Square);
        }

        private void btnSemicerchioSopra_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.SemicerchioSopra);
        }

        private void btnSemicerchioSotto_Click(object sender, EventArgs e)
        {
            CreaFigura2Coordinate(MacroGeometrySelectionMode.SemicerchioSotto);
        }

       



        #endregion COMANDI_2_COORDINATE

        #region COMANDI_PIU_COORDINATE
        #region DGV
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Rimuove eventuali handler precedenti per evitare duplicazioni
            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= Tb_KeyPress;
                tb.KeyPress += Tb_KeyPress;
            }
        }

        private void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permetti: cifre, segno meno, separatore decimale, backspace
            char sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '-' &&
                e.KeyChar != sep)
            {
                e.Handled = true; // blocca il carattere
            }

            // Il '-' solo all'inizio
            if (e.KeyChar == '-' && (sender as TextBox).SelectionStart != 0)
            {
                e.Handled = true;
            }

            // Un solo separatore decimale
            if (e.KeyChar == sep && (sender as TextBox).Text.Contains(sep))
            {
                e.Handled = true;
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Salta la riga "nuova" vuota
            if (dataGridView1.Rows[e.RowIndex].IsNewRow) return;

            string value = e.FormattedValue?.ToString() ?? "";

            // Se la cella è vuota, va bene (verrà controllato in fase di lettura)
            if (string.IsNullOrWhiteSpace(value)) return;

            if (!double.TryParse(value, out _))
            {
                MessageBox.Show("Inserisci solo valori numerici validi.",
                    "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }
        #endregion DGV

        public List<(int X, int Y)> LeggiPunti()
        {
            var risultato = new List<(int X, int Y)>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Salta la riga "nuova riga" vuota
                if (row.IsNewRow) continue;

                // Salta eventuali righe interamente vuote
                if (row.Cells[0].Value == null && row.Cells[1].Value == null)
                    continue;

                string sx = row.Cells[0].Value?.ToString()?.Trim() ?? "";
                string sy = row.Cells[1].Value?.ToString()?.Trim() ?? "";

                // Controllo: entrambe valorizzate
                if (string.IsNullOrWhiteSpace(sx) || string.IsNullOrWhiteSpace(sy))
                {
                    throw new InvalidOperationException(
                        $"La riga {row.Index + 1} non ha entrambe le celle valorizzate (X e Y).");
                }

                // Controllo: numeriche
                if (!int.TryParse(sx, out int x))
                    throw new InvalidOperationException(
                        $"Il valore X alla riga {row.Index + 1} non è un numero valido.");

                if (!int.TryParse(sy, out int y))
                    throw new InvalidOperationException(
                        $"Il valore Y alla riga {row.Index + 1} non è un numero valido.");

                risultato.Add((x, y));
            }

            if (risultato.Count == 0)
                throw new InvalidOperationException("Nessun dato inserito.");

            return risultato;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Rimuove tutte le righe (comprese eventuali righe vuote)
            dataGridView1.Rows.Clear();

            // Se l'utente può aggiungere righe, ne lasciamo una vuota pronta
            if (dataGridView1.AllowUserToAddRows)
            {
                dataGridView1.Rows.Add();
            }

            // Rimuove eventuali selezioni residue
            dataGridView1.ClearSelection();
        }

        private void CreaFiguraPiuCoordinate(MacroGeometrySelectionMode pGeometry)
        {
            try
            {
                punti = LeggiPunti();

                if (punti.Count > 1)
                {
                    //seleziona il tool prescelto
                    MAs.GeometryModeSelected = pGeometry;

                    //crea la figura e il cursore
                    MAs.CreateFigure(MAs.GeometryModeSelected);


                    int ptsTotali = punti.Count;
                    int conta = 0;
                    foreach (var item in punti)
                    {
                        conta++;

                        if (conta == 1)
                        {
                            MouseEventArgs e0 = new MouseEventArgs(MouseButtons.Left, 1, item.X, item.Y, 0);
                            Keys ModifierKeys = new Keys();
                            MAs.Canvas_MouseDown(e0, ModifierKeys);
                        }
                        else if (conta > 1 && conta < ptsTotali)
                        {
                            MouseEventArgs e0 = new MouseEventArgs(MouseButtons.Left, 1, item.X, item.Y, 0);
                            Keys ModifierKeys = new Keys();
                            MAs.Canvas_MouseDown(e0, ModifierKeys);
                            MAs.Canvas_MouseMove(e0, ModifierKeys);
                            MAs.Canvas_MouseUp(e0, ModifierKeys);
                        }
                        else
                        {
                            MouseEventArgs e0 = new MouseEventArgs(MouseButtons.Left, 1, item.X, item.Y, 0);
                            Keys ModifierKeys = new Keys();
                            MAs.Canvas_MouseDown(e0, ModifierKeys);
                            MAs.Canvas_MouseMove(e0, ModifierKeys);
                            MAs.Canvas_MouseUp(e0, ModifierKeys);
                            MouseEventArgs e1 = new MouseEventArgs(MouseButtons.Left, 2, item.X, item.Y, 0);
                            MAs.Canvas_MouseDown(e1, ModifierKeys);
                            MAs.Canvas_MouseMove(e1, ModifierKeys);
                            MAs.Canvas_MouseUp(e1, ModifierKeys);
                        }



                    }
                    MAs.GeometryModeSelected = MacroGeometrySelectionMode.None;
                }





            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Errore di validazione",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPolyline_Click(object sender, EventArgs e)
        {
            CreaFiguraPiuCoordinate(MacroGeometrySelectionMode.Polyline);
        }

        private void btnBezier_Click(object sender, EventArgs e)
        {
            CreaFiguraPiuCoordinate(MacroGeometrySelectionMode.Bezier);
        }
        private void btnPolylinePalleggio_Click(object sender, EventArgs e)
        {
            CreaFiguraPiuCoordinate(MacroGeometrySelectionMode.PalleggioBasket);
        }
        #endregion COMANDI_PIU_COORDINATE

        private void btnTitle_Click(object sender, EventArgs e)
        {
            if (MAs.GeometryModeSelected != MacroGeometrySelectionMode.None && !string.IsNullOrEmpty(txtTitle.Text))
            {
                MAs.SetSelectionContainerGeometryTitle(txtTitle.Text.Trim());
            }
        }

    
    }
}

