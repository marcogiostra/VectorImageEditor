using EditorModel.Common;
using EditorModel.Figures.Basket;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace EditorModel.Figures.Extentions
{

    public enum CourtyCampionato
    {
        NONE = 0,
        FIBA = 1,
        NBA = 2,
        WNBA = 3,
        NCAA_M = 4,
        NCAA_W = 5
    }

    public enum BasketCourtObjectType
    {
        None ,

        CanestroSoloAttacco ,
        CanestroSoloDifesa,
        CanestroFullDifesa,
        CanestroFullDifesaPalazzetto,

        LineaNoSfondoAttacco,
        LineaNoSfondoDifesa,
        LineaNoSfondoDifesaFull,
        LineaNoSfondoDifesaFullPalazzetto,

        TaccheAttacco,
        TaccheDifesa,
        TaccheDifesaFull,
        TaccheDifesaFullPalazzetto,

        LineeFuoriCampo,
        HasMarks,
        
        LineaRimesseEsterneAttacco,
        LineaRimesseEsterneDifessa,
        LineaRimesseEsterneFull,
        LineaRimesseEsterneFullPalazzetto,

        LineaRimesseInterneSoloAttacco,
        LineaRimesseInterneSoloDifesa,
        LineaRimesseInterneFull,
        LineaRimesseInterneFullPalazzetto,


        LineaPanchineFull,
        LineaPanchineAttacco,
        LineaPanchineDifesa,

        ZonaCambiSoloAttacco,
        ZonaCambiSoloDifesa,
        ZonaCambiFull,

        //disegna la Linea da tre punti
        LineaTiroTrePunti,
    }

    /// <summary>
    /// EXTENSIONI PER IL PATH BASKET
    /// </summary>
    public static partial class GraphicsPathExtensions
    {

        #region BASKET

        #region OGGETTI_CAMPO_BASKET
        public static void AddOggettoCampoBasketAttacco(this GraphicsPath path, CourtyCampionato camp, bool FullCamp,bool DrawPalazzetto, BasketCourtObjectType tipoOggetto, RectangleF rect)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            AddOggettoCampoBasketAttacco(path, camp, tipoOggetto, rect.X, rect.Y, rect.Width, rect.Height, FullCamp, DrawPalazzetto);
        }
        public static void AddOggettoCampoBasketAttacco(this GraphicsPath path, CourtyCampionato camp, BasketCourtObjectType tipoOggetto, float x0, float y0, float larghezza, float altezza, bool FullCamp, bool DrawPalazzetto)
        {
            if (tipoOggetto != BasketCourtObjectType.LineaTiroTrePunti)
                DrawBasketCourtObject(path, camp, x0, y0, larghezza, altezza, rotated180: false, tipoOggetto, FullCamp, DrawPalazzetto);
            else
                Draw3ptsLine(path, camp, x0, y0, larghezza, altezza, ruota180: false);
        }

        public static void AddOggettoCampoBasketDifesa(this GraphicsPath path, CourtyCampionato camp, bool FullCamp, bool DrawPalazzetto, BasketCourtObjectType tipoOggetto, RectangleF rect)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            AddOggettoCampoBasketDifesa(path, camp, tipoOggetto, rect.X, rect.Y, rect.Width, rect.Height, FullCamp, DrawPalazzetto);
        }
        public static void AddOggettoCampoBasketDifesa(this GraphicsPath path, CourtyCampionato camp, BasketCourtObjectType tipoOggetto, float x0, float y0, float larghezza, float altezza, bool FullCamp, bool DrawPalazzetto)
        {
            if (tipoOggetto != BasketCourtObjectType.LineaTiroTrePunti)
                DrawBasketCourtObject(path, camp, x0, y0, larghezza, altezza, rotated180: true, tipoOggetto, FullCamp, DrawPalazzetto);
            else
                Draw3ptsLine(path, camp, x0, y0, larghezza, altezza, ruota180: true);
        }

        private static void DrawBasketCourtObject(this GraphicsPath path, CourtyCampionato camp, float x0, float y0, float larghezza, float altezza, bool rotated180, BasketCourtObjectType TypoOggetto, bool IsFullCamp, bool DrawPalazzetto)
        {
            bool CanestroSoloAttacco = TypoOggetto == BasketCourtObjectType.CanestroSoloAttacco;
            bool CanestroSoloDifesa = TypoOggetto == BasketCourtObjectType.CanestroSoloDifesa;
            bool CanestroFullDifesa = TypoOggetto == BasketCourtObjectType.CanestroFullDifesa;
            bool CanestroFullDifesaPalazzetto = TypoOggetto == BasketCourtObjectType.CanestroFullDifesaPalazzetto;

            bool LineaNoSfondoAttacco = TypoOggetto == BasketCourtObjectType.LineaNoSfondoAttacco;
            bool LineaNoSfondoDifesa = TypoOggetto == BasketCourtObjectType.LineaNoSfondoDifesa;
            bool LineaNoSfondoDifesaFull = TypoOggetto == BasketCourtObjectType.LineaNoSfondoDifesaFull;
            bool LineaNoSfondoDifesaFullPalazzetto = TypoOggetto == BasketCourtObjectType.LineaNoSfondoDifesaFullPalazzetto;

            bool lineeFuoriCampo = TypoOggetto == BasketCourtObjectType.LineeFuoriCampo;
            
            bool TaccheAttacco = TypoOggetto == BasketCourtObjectType.TaccheAttacco;
            bool TaccheDifesa = TypoOggetto == BasketCourtObjectType.TaccheDifesa;
            bool TaccheDifesaFull = TypoOggetto == BasketCourtObjectType.TaccheDifesaFull;
            bool TaccheDifesaFullPalazzetto = TypoOggetto == BasketCourtObjectType.TaccheDifesaFullPalazzetto;

            bool HasMarks = TypoOggetto == BasketCourtObjectType.HasMarks;
            
            bool LineaRimesseEsterneFull = TypoOggetto == BasketCourtObjectType.LineaRimesseEsterneFull;
            bool LineaRimesseEsterneFullPalazzetto = TypoOggetto == BasketCourtObjectType.LineaRimesseEsterneFullPalazzetto;
            bool LineaRimesseEsterneAttacco = TypoOggetto == BasketCourtObjectType.LineaRimesseEsterneAttacco;
            bool LineaRimesseEsterneDifessa = TypoOggetto == BasketCourtObjectType.LineaRimesseEsterneDifessa;

            bool LineaRimesseInterneSoloAttacco = TypoOggetto == BasketCourtObjectType.LineaRimesseInterneSoloAttacco;
            bool LineaRimesseInterneSoloDifesa = TypoOggetto == BasketCourtObjectType.LineaRimesseInterneSoloDifesa;
            bool LineaRimesseInterneFull = TypoOggetto == BasketCourtObjectType.LineaRimesseInterneFull;
            bool LineaRimesseInterneFullPalazzetto = TypoOggetto == BasketCourtObjectType.LineaRimesseInterneFullPalazzetto;
            
            bool LineaPanchineFull = TypoOggetto == BasketCourtObjectType.LineaPanchineFull;
            bool LineaPanchineAttacco = TypoOggetto == BasketCourtObjectType.LineaPanchineAttacco;
            bool LineaPanchineDifesa = TypoOggetto == BasketCourtObjectType.LineaPanchineDifesa;

            bool ZonaCambiSoloAttacco = TypoOggetto == BasketCourtObjectType.ZonaCambiSoloAttacco;
            bool ZonaCambiSoloDifesa = TypoOggetto == BasketCourtObjectType.ZonaCambiSoloDifesa;
            bool ZonaCambiFull = TypoOggetto == BasketCourtObjectType.ZonaCambiFull;


            SpecificheCampoF spec = default;
            
            if (camp == CourtyCampionato.FIBA)
                spec = BasketSpecifiche.FIBA_ORIG;
            else if (camp == CourtyCampionato.NBA)
                spec = BasketSpecifiche.NBA_ORIG;
            else if (camp == CourtyCampionato.WNBA)
                spec = BasketSpecifiche.WNBA_ORIG;
            else if (camp == CourtyCampionato.NCAA_M)
                spec = BasketSpecifiche.NCAA_M_ORIG;
            else if (camp == CourtyCampionato.NCAA_W)
                spec = BasketSpecifiche.NCAA_W_ORIG;

            PointF start = new PointF(x0, y0);
            PointF current = new PointF(x0 + larghezza, y0 + altezza);
            float scaleA1 = 1;
            float runOffL = 0;

            //perosnalizzazioni campi comuni
            float linea = spec.SpessoreLineeCampo;
            float tabelloneToBaseline = spec.Tabellone_y0;
            float tabelloWidth = spec.TabelloneLarghezza;
            float canestroToBaseline = spec.CentroCanestroY;
            float raggioNoSfondoLine = spec.NoSfondoRaggio;



            //da personalizzare
            float mezzoCampoHeight = spec.MetaLengh;
            float fullCampoWidth = spec.FullWidth;
            float fullCampoHeight = spec.FullLength;
            float noSfondamentoRaggio = spec.NoSfondoRaggio;
            float areaWith = spec.AreaLarghezza;
            float areaHeight = spec.AreaLunghezza;
            float raggioLunetta = spec.LunettaRaggio;
            float[] ReboundTacche = spec.ReboundTacche;

            //float[] ReboundTaccheInterne = spec.ReboundTaccheInterne;
            float centerX = spec.FullWidth / 2f;
            //float areaInternaWith = spec.AreaPiccolaLarghezza;
            (PointF p1, PointF p2)[] hasMarks = spec.HasMarks;
            
            //linee rimesse esterne
            (PointF p1, PointF p2)[] lineaRimesseEsterneFull_F = spec.LineaRimesseEsterneFull;
            (PointF p1, PointF p2)[] LineaRimesseEsterneFullPalazzetto_F = spec.LineaRimesseEsterneFullPalazzetto;
            (PointF p1, PointF p2)[] lineaRimesseEsterneAttacco_F = spec.LineaRimesseEsterneSoloAttacco;
            (PointF p1, PointF p2)[] lineaRimesseEsterneDifesa_F = spec.LineaRimesseEsterneSoloDifesa;
            

            (PointF p1, PointF p2)[] LineaRimesseInterneSoloAttacco_F = spec.LineaRimesseInterneSoloAttacco;
            (PointF p1, PointF p2)[] LineaRimesseInterneSoloDifesa_F = spec.LineaRimesseInterneSoloDifesa;
            (PointF p1, PointF p2)[] LineaRimesseInterneFull_F = spec.LineaRimesseInterneFull;
            (PointF p1, PointF p2)[] LineaRimesseInterneFullPalazzetto_F = spec.LineaRimesseInterneFullPalazzetto;
            //
            //linea panchine
            (PointF p1, PointF p2)[] LineaPanchineAttacco_F = spec.LineaPanchineSoloAttacco;
            (PointF p1, PointF p2)[] LineaPanchineDifesa_F = spec.LineaPanchineSoloDifesa;
            (PointF p1, PointF p2)[] LineaPanchineFull_F = spec.LineaPanchineFull;
            //
            (PointF p1, PointF p2)[] ZonaCambiSoloAttaccoF = spec.ZonaCambiSoloAttacco;
            (PointF p1, PointF p2)[] ZonaCambiSoloDifesaF = spec.ZonaCambiSoloDifesa;
            (PointF p1, PointF p2)[] ZonaCambiFullF = spec.ZonaCambiFull;
            
            #region CALCOLI_COORDINATE

            float dispW = current.X - start.X;
            float dispH = current.Y - start.Y;
            if (dispW <= 0 || dispH <= 0) return;

            float totalBoundingW = fullCampoWidth + (runOffL > 0 ? runOffL * 2 : 0);
            float totalBoundingH = (fullCampoHeight / 2f) + (runOffL > 0 ? runOffL : 0);
            float scalaProporzionale = Math.Min(dispW / totalBoundingW, dispH / totalBoundingH) * scaleA1;

            float offsetX = runOffL > 0 ? runOffL : 0;
            float offsetY = runOffL > 0 ? runOffL : 0;

            #region TOSCREEN_CALCULATE
            PointF ToScreen(float cmX, float cmY)
            {
                float finalX = start.X + (cmX + offsetX) * scalaProporzionale;
                float finalY = start.Y + (cmY + offsetY) * scalaProporzionale;
                return new PointF(finalX, finalY);
            }

            PointF ToScreenPalazzetto(float cmX, float cmY)
            {
                // Tutte le varianti delle linee panchina sono definite nel
                // riquadro esterno completo, con 200 cm di fascia per lato.
                // Anche le varianti Attacco/Difesa vengono disegnate in questo
                // riquadro: la scala della sola metà campo le porta fuori sede.
                float frameWidth = fullCampoWidth + 400f;
                float frameHeight = fullCampoHeight + 400f;
                return new PointF(
                    start.X + cmX / frameWidth * dispW,
                    start.Y + cmY / frameHeight * dispH);
            }

            PointF ToScreenPalazzettoDifesa(float cmX, float cmY)
            {
                // Nel disegno Difesa il campo inizia a Y=0 e il riquadro non
                // include il margine verticale del palazzetto.
                float frameWidth = fullCampoWidth + 400f;
                return new PointF(
                    start.X + cmX / frameWidth * dispW,
                    start.Y + cmY / fullCampoHeight * dispH);
            }

            PointF ToScreenPalazzettoInterno(float cmX, float cmY)
            {
                // Tutte le varianti delle linee panchina sono definite nel
                // riquadro esterno completo, con 200 cm di fascia per lato.
                // Anche le varianti Attacco/Difesa vengono disegnate in questo
                // riquadro: la scala della sola metà campo le porta fuori sede.
                float frameWidth = fullCampoWidth + 200f;
                float frameHeight = fullCampoHeight + 200f;
                return new PointF(
                    start.X + cmX / frameWidth * dispW,
                    start.Y + cmY / frameHeight * dispH);
            }


            PointF ToScreenFull(float cmX, float cmY)
            {
                // Tutte le varianti delle linee panchina sono definite nel
                // riquadro esterno completo, con 200 cm di fascia per lato.
                // Anche le varianti Attacco/Difesa vengono disegnate in questo
                // riquadro: la scala della sola metà campo le porta fuori sede.
                float frameWidth = fullCampoWidth;
                float frameHeight = fullCampoHeight;
                return new PointF(
                    start.X + cmX / frameWidth * dispW,
                    start.Y + cmY / frameHeight * dispH);
            }
            #endregion TOSCREEN_CALCULATE


            //per tacche
            float xKeyLeft = spec.Tacche_x0;
            float xKeyRight = spec.Tacche_x1;
            //float yKeyTop = linea + areaHeight;          
            #endregion CALCOLI_COORDINATE


            path.StartFigure();

            if(HasMarks)
            {
                foreach (var mark in hasMarks)
                {
                    PointF p1 = ToScreen(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreen(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }

            if (ZonaCambiSoloAttacco)
            {
                foreach (var mark in ZonaCambiSoloAttaccoF)
                {
                    PointF p1 = DrawPalazzetto
                        ? ToScreenPalazzetto(mark.p1.X, mark.p1.Y)
                        : ToScreenFull(mark.p1.X, mark.p1.Y);
                    PointF p2 = DrawPalazzetto
                        ? ToScreenPalazzetto(mark.p2.X, mark.p2.Y)
                        : ToScreenFull(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }

            if (ZonaCambiSoloDifesa)
            {
                foreach (var mark in ZonaCambiSoloDifesaF)
                {
                    PointF p1 = DrawPalazzetto
                        ? ToScreenPalazzetto(mark.p1.X, mark.p1.Y)
                        : ToScreenFull(mark.p1.X, mark.p1.Y);
                    PointF p2 = DrawPalazzetto
                        ? ToScreenPalazzetto(mark.p2.X, mark.p2.Y)
                        : ToScreenFull(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }

            if (ZonaCambiFull)
            {
                foreach (var mark in ZonaCambiFullF)
                {
                    PointF p1 = DrawPalazzetto
                        ? ToScreenPalazzetto(mark.p1.X, mark.p1.Y)
                        : ToScreenFull(mark.p1.X, mark.p1.Y);
                    PointF p2 = DrawPalazzetto
                        ? ToScreenPalazzetto(mark.p2.X, mark.p2.Y)
                        : ToScreenFull(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }

        


         

 
            #region LINEA_RIMESSE_INTERNE
            if(LineaRimesseInterneSoloAttacco)
            {
                foreach (var mark in LineaRimesseInterneSoloAttacco_F)
                {
                    PointF p1 = ToScreen(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreen(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            if (LineaRimesseInterneSoloDifesa)
            {
                foreach (var mark in LineaRimesseInterneSoloDifesa_F)
                {
                    PointF p1 = ToScreen(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreen(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            if (LineaRimesseInterneFull)
            {
                foreach (var mark in LineaRimesseInterneFull_F)
                {
                    PointF p1 = ToScreen(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreen(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            if (LineaRimesseInterneFullPalazzetto)
            {
                foreach (var mark in LineaRimesseInterneFullPalazzetto_F)
                {
                    PointF p1 = ToScreenPalazzetto(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreenPalazzetto(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            #endregion LINEA_RIMESSE_INTERNE

            #region LINEA_RIMESSE_ESTERNE
            if (LineaRimesseEsterneFull)
            {
                foreach (var mark in lineaRimesseEsterneFull_F)
                {
                    PointF p1 = ToScreenPalazzetto(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreenPalazzetto(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            if (LineaRimesseEsterneFullPalazzetto)
            {
                foreach (var mark in LineaRimesseEsterneFullPalazzetto_F)
                {
                    PointF p1 = ToScreenPalazzetto(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreenPalazzetto(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            if (LineaRimesseEsterneAttacco)
            {
                foreach (var mark in lineaRimesseEsterneAttacco_F)
                {
                    PointF p1 = ToScreenPalazzetto(mark.p1.X, mark.p1.Y);
                    PointF p2 = ToScreenPalazzetto(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            if (LineaRimesseEsterneDifessa)
            {
                foreach (var mark in lineaRimesseEsterneDifesa_F)
                {
                    PointF p1 = DrawPalazzetto
                        ? ToScreenPalazzettoDifesa(mark.p1.X, mark.p1.Y)
                        : ToScreenFull(mark.p1.X, mark.p1.Y);
                    PointF p2 = DrawPalazzetto
                        ? ToScreenPalazzettoDifesa(mark.p2.X, mark.p2.Y)
                        : ToScreenFull(mark.p2.X, mark.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }
            #endregion LINEA_RIMESSE_ESTERNE

            #region LINEA_PANCHINE
            if (LineaPanchineFull)
            {
                foreach (var lin in LineaPanchineFull_F)
                {
                    PointF p1 = ToScreenPalazzetto(lin.p1.X, lin.p1.Y);
                    PointF p2 = ToScreenPalazzetto(lin.p2.X, lin.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }

            if (LineaPanchineAttacco)
            {
                foreach (var lin in LineaPanchineAttacco_F)
                {
                    PointF p1 = ToScreenPalazzetto(lin.p1.X, lin.p1.Y);
                    PointF p2 = ToScreenPalazzetto(lin.p2.X, lin.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);
                }
            }

            if (LineaPanchineDifesa)
            {
                foreach (var lin in LineaPanchineDifesa_F)
                {
                    PointF p1 = DrawPalazzetto
                        ? ToScreenPalazzettoDifesa(lin.p1.X, lin.p1.Y)
                        : ToScreenFull(lin.p1.X, lin.p1.Y);
                    PointF p2 = DrawPalazzetto
                        ? ToScreenPalazzettoDifesa(lin.p2.X, lin.p2.Y)
                        : ToScreenFull(lin.p2.X, lin.p2.Y);
                    path.StartFigure();
                    path.AddLine(p1, p2);

                }
            }
            #endregion LINEA_PANCHINE

            #region TACCHE_RIMBALZO
            if (TaccheAttacco || TaccheDifesa || TaccheDifesaFull || TaccheDifesaFullPalazzetto)
            {
                float sporge = 10f;

                if (camp == CourtyCampionato.FIBA && ReboundTacche != null && ReboundTacche.Length >= 5)
                {
                    //primo
                    float yAssoluta = linea + ReboundTacche[0];
                    // Tacchetta sinistra 
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                    // Tacchetta destra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));

                    //secondo+terzo = rettangolo
                    for (float yy = ReboundTacche[1]; yy <= ReboundTacche[2]; yy++)
                    {
                        yAssoluta = linea + yy;
                        // Tacchetta sinistra (sporge di 5 cm o 2 pollici verso l'esterno)
                        path.StartFigure();
                        path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                        // Tacchetta destra
                        path.StartFigure();
                        path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));
                    }

                    //quarta
                    yAssoluta = linea + ReboundTacche[3];
                    // Tacchetta sinistra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                    // Tacchetta destra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));

                    //quinta
                    yAssoluta = linea + ReboundTacche[4];
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                    // Tacchetta destra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));


                }
                else if ((camp == CourtyCampionato.NCAA_M || camp == CourtyCampionato.NCAA_W)
                    && ReboundTacche != null && ReboundTacche.Length >= 5)
                {
                    float yAssoluta = 0;
                    //primo e secondo = rettangolo
                    for (float yy = ReboundTacche[0]; yy <= ReboundTacche[1]; yy++)
                    {
                        yAssoluta = linea + yy;
                        // Tacchetta sinistra (sporge di 5 cm o 2 pollici verso l'esterno)
                        path.StartFigure();
                        path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                        // Tacchetta destra
                        path.StartFigure();
                        path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));
                    }

                    //terzo
                    yAssoluta = linea + ReboundTacche[2];
                    // Tacchetta sinistra 
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                    // Tacchetta destra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));



                    //quarta
                    yAssoluta = linea + ReboundTacche[3];
                    // Tacchetta sinistra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                    // Tacchetta destra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));

                    //quinta
                    yAssoluta = linea + ReboundTacche[4];
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                    // Tacchetta destra
                    path.StartFigure();
                    path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));


                }
                else if (ReboundTacche != null)
                {
                    foreach (float yNotchCm in ReboundTacche)
                    {
                        float yAssoluta = linea + yNotchCm;
                        // Tacchetta sinistra (sporge di 5 cm o 2 pollici verso l'esterno)
                        path.StartFigure();
                        path.AddLine(ToScreen(xKeyLeft, yAssoluta), ToScreen(xKeyLeft - sporge, yAssoluta));
                        // Tacchetta destra
                        path.StartFigure();
                        path.AddLine(ToScreen(xKeyRight, yAssoluta), ToScreen(xKeyRight + sporge, yAssoluta));
                    }

                }
            }

            #endregion TACCHE_RIMBALZO

            #region TABELLONME+CANESTRO
            //tabellone e canestro 
            float yBoard = tabelloneToBaseline;
            float xBoardLeft = centerX - (tabelloWidth / 2f);
            float xBoardRight = centerX + (tabelloWidth / 2f);
            float yBasket = canestroToBaseline; // Y centro canestro
            PointF cBasketScrn = ToScreen(centerX, yBasket);
            float rBasketScrn = 22.85f * scalaProporzionale;
            if (CanestroSoloAttacco || CanestroSoloDifesa || CanestroFullDifesa || CanestroFullDifesaPalazzetto)
            {
                path.StartFigure();
                path.AddLine(ToScreen(xBoardLeft, yBoard), ToScreen(xBoardRight, yBoard)); // Tratto vettoriale del tabellone
                                                                                           //                                                                         //
                path.StartFigure();
                path.AddEllipse(new RectangleF(cBasketScrn.X - rBasketScrn, cBasketScrn.Y - rBasketScrn, rBasketScrn * 2, rBasketScrn * 2));
            }
            #endregion TABELLONME+CANESTRO


            //semicerchio di sfondamento
            PointF cBoardScrn = ToScreen(centerX, yBoard);
            float rNoChargeScrn = raggioNoSfondoLine * scalaProporzionale;
            RectangleF boxNoCharge = new RectangleF(
                cBoardScrn.X - rNoChargeScrn,
                cBoardScrn.Y - rNoChargeScrn,
                rNoChargeScrn * 2f,
                rNoChargeScrn * 2f);
            if (LineaNoSfondoAttacco || LineaNoSfondoDifesa || LineaNoSfondoDifesaFull || LineaNoSfondoDifesaFullPalazzetto)
            {
                path.StartFigure();
                path.AddArc(boxNoCharge, 0f, 180f); // Rivolto verso la metà campo partendo dall'asse del tabellone
            }
     




            // 8. FUORI CAMPO (RUN-OFF) E RIMESSE
            if (runOffL > 0)
            {
                path.StartFigure();
                path.AddLine(ToScreen(-runOffL, -runOffL), ToScreen(fullCampoWidth + runOffL, -runOffL));
                path.AddLine(ToScreen(fullCampoWidth + runOffL, -runOffL), ToScreen(fullCampoWidth + runOffL, fullCampoHeight / 2f));
                path.StartFigure();
                path.AddLine(ToScreen(-runOffL, -runOffL), ToScreen(-runOffL, fullCampoHeight / 2f));

                float yRimessa = fullCampoHeight == 2810f ? 832.5f : 853.44f;
                if (yRimessa <= fullCampoHeight / 2f)
                {
                    if (lineeFuoriCampo)
                    {
                        path.StartFigure();
                        path.AddLine(ToScreen(-15f, yRimessa), ToScreen(0, yRimessa));
                        path.StartFigure();
                        path.AddLine(ToScreen(fullCampoWidth, yRimessa), ToScreen(fullCampoWidth + 15f, yRimessa));
                    }
                }
            }

            // --- NUOVA LOGICA DI ROTAZIONE VETTORIALE ---
            if (rotated180)
            {
                // Calcoliamo il centro geometrico del rettangolo in cui l'oggetto è disegnato
                float centroMondoX = x0 + (larghezza / 2f);
                float centroMondoY = y0 + (altezza / 2f);
                PointF centroRotazione = new PointF(centroMondoX, centroMondoY);

                // Usiamo la matrice di trasformazione per ruotare i vettori del tracciato di 180 gradi
                using (Matrix matriceRotazione = new Matrix())
                {
                    matriceRotazione.RotateAt(180f, centroRotazione);
                    path.Transform(matriceRotazione); // Applica la rotazione a tutti i punti interni
                }
            }


  


            if (!LineaNoSfondoAttacco
                && !LineaNoSfondoDifesa
                && !LineaNoSfondoDifesaFull
                && !LineaNoSfondoDifesaFullPalazzetto)
                path.CloseFigure();

        }

        private static void Draw3ptsLine(this GraphicsPath path, CourtyCampionato camp, float X0, float Y0, float larghezza, float altezza, bool ruota180)
        {
            float X_START = 0f;
            float X_END = 0f;
            float Y_BOTTOM = 0f;
            float Y_TOP_ARC = 0f;
            float Y_STRAIGHT_END = 0f;
            float CENTER_X = 0f;
            float CENTER_Y = 0f;
            float RADIUS = 0f;
            float WIDTH = X_END - X_START;
            float HEIGHT = Y_TOP_ARC - Y_BOTTOM;
            float angoloInizio = 0f;
            float angoloAmpiezza = 0f;

            if (camp == CourtyCampionato.FIBA)
            {
                X_START = 95f;
                X_END = 1415f;
                Y_BOTTOM = 5f;
                Y_TOP_ARC = 834f; //837.5f;                    
                Y_STRAIGHT_END = 204f;
                CENTER_X = 755f;
                CENTER_Y = 160f;
                RADIUS = 675f;
                WIDTH = X_END - X_START;
                HEIGHT = Y_TOP_ARC - Y_BOTTOM;
                angoloInizio = 12.2f;
                angoloAmpiezza = 155.6f;
            }
            else if (camp == CourtyCampionato.NBA)
            {
                X_START = 96f;
                X_END = 1438f;
                Y_BOTTOM = 5.08f;
                Y_TOP_ARC = 851f;
                Y_STRAIGHT_END = 395f;
                CENTER_X = 765F;
                CENTER_Y = 127f;
                RADIUS = 724;
                WIDTH = X_END - X_START;
                HEIGHT = Y_TOP_ARC - Y_BOTTOM;
                angoloInizio = 22.2f;
                angoloAmpiezza = 135.6f;
            }
            else if (camp == CourtyCampionato.WNBA)
            {
                X_START = 96f;
                X_END = 1438f;
                Y_BOTTOM = 5.08f;
                Y_TOP_ARC = 851f;
                Y_STRAIGHT_END = 125F;
                CENTER_X = 765F;
                CENTER_Y = 127f;
                RADIUS = 675;
                WIDTH = X_END - X_START;
                HEIGHT = Y_TOP_ARC - Y_BOTTOM;
                angoloInizio = 11.8f;
                angoloAmpiezza = 156.4f;
            }
            else if (camp == CourtyCampionato.NCAA_M)
            {
                X_START = 95f;
                X_END = 1415f;
                Y_BOTTOM = 5f;
                Y_TOP_ARC = 837.5f;
                Y_STRAIGHT_END = 120f;                  //304.2f;
                CENTER_X = 755f;
                CENTER_Y = 162.5f;
                RADIUS = 675f;
                WIDTH = X_END - X_START;
                HEIGHT = Y_TOP_ARC - Y_BOTTOM;
                angoloInizio = 12.2f;
                angoloAmpiezza = 155.6f;
            }
            else if (camp == CourtyCampionato.NCAA_W)
            {
                X_START = 95f;
                X_END = 1415f;
                Y_BOTTOM = 5f;
                Y_TOP_ARC = 837.5f;
                Y_STRAIGHT_END = 120f;                  //304.2f;
                CENTER_X = 755f;
                CENTER_Y = 162.5f;
                RADIUS = 675f;
                WIDTH = X_END - X_START;
                HEIGHT = Y_TOP_ARC - Y_BOTTOM;
                angoloInizio = 12.2f;
                angoloAmpiezza = 155.6f;
            }
            else
                return;

            float rapportoInizialeA1 = 1f;
            PointF startPoint = new PointF(X0, Y0);
            PointF currentPoint = new PointF(X0 + larghezza, Y0 + altezza);

            float disponibileWidth = currentPoint.X - startPoint.X;
            float disponibileHeight = currentPoint.Y - startPoint.Y;
            if (disponibileWidth <= 0 || disponibileHeight <= 0)
                return;

            float scalaX = disponibileWidth / WIDTH;
            float scalaY = disponibileHeight / HEIGHT;
            float scalaProporzionale = Math.Min(scalaX, scalaY) * rapportoInizialeA1;

            PointF ToScreen(float cmX, float cmY)
            {
                float riscaldatoX = startPoint.X + (cmX - X_START) * scalaProporzionale;
                float riscaldatoY = startPoint.Y + (cmY - Y_BOTTOM) * scalaProporzionale;
                return new PointF(riscaldatoX, riscaldatoY);
            }

            PointF pInizioSinistro = ToScreen(X_START, Y_BOTTOM);
            PointF pInizioDestro = ToScreen(X_END, Y_BOTTOM);
            PointF pFineDrittoDestro = ToScreen(X_END, Y_STRAIGHT_END);
            PointF pFineDrittoSinistro = ToScreen(X_START, Y_STRAIGHT_END);

            PointF centroCanestro = ToScreen(CENTER_X, CENTER_Y);
            float raggioRiscaldato = RADIUS * scalaProporzionale;

            // Creiamo un path temporaneo per generare la figura base rivolta verso il basso
            using (GraphicsPath localPath = new GraphicsPath())
            {
                // Segmento 1: Linea di fondo
                localPath.AddLine(pInizioSinistro, pInizioDestro);

                // Segmento 2: Tratto rettilineo destro
                localPath.AddLine(pInizioDestro, pFineDrittoDestro);


                // Segmento 3: L'arco da tre punti centrale
                RectangleF boundingBoxCerchio = new RectangleF(
                    centroCanestro.X - raggioRiscaldato,
                    centroCanestro.Y - raggioRiscaldato,
                    raggioRiscaldato * 2,
                    raggioRiscaldato * 2
                );
                localPath.AddArc(boundingBoxCerchio, angoloInizio, angoloAmpiezza);


                // Segmento 4: Tratto rettilineo sinistro
                localPath.AddLine(pFineDrittoSinistro, pInizioSinistro);

                localPath.CloseFigure();

                // --- NUOVA LOGICA DI ROTAZIONE VETTORIALE ---
                if (ruota180)
                {
                    // Calcoliamo il centro geometrico del rettangolo in cui l'oggetto è disegnato
                    float centroMondoX = X0 + (larghezza / 2f);
                    float centroMondoY = Y0 + (altezza / 2f);
                    PointF centroRotazione = new PointF(centroMondoX, centroMondoY);

                    // Usiamo la matrice di trasformazione per ruotare i vettori del tracciato di 180 gradi
                    using (Matrix matriceRotazione = new Matrix())
                    {
                        matriceRotazione.RotateAt(180f, centroRotazione);
                        localPath.Transform(matriceRotazione); // Applica la rotazione a tutti i punti interni
                    }
                }

                // Aggiungiamo le linee trasformate al path principale passato come parametro alla funzione
                path.AddPath(localPath, false);
            }
        }



        #endregion OGGETTI_CAMPO_BASKET

        #endregion BASKET


    }


    public static partial class GraphicsPathExtensions
    {
       
        public static void AddSinuousBasketLines(this GraphicsPath path, PointF[] punti, bool chiudi = false, float ampiezza = 0.035f, float ondePerSegmento = 4.0f, int passiPerSegmento = 64)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (punti == null)
                throw new ArgumentNullException(nameof(punti));

            if (punti.Length < 2)
                return;

            if (ampiezza < 0f)
                throw new ArgumentException(
                    "ampiezza deve essere >= 0.",
                    nameof(ampiezza));

            if (ondePerSegmento < 0f)
                throw new ArgumentException(
                    "ondePerSegmento deve essere >= 0.",
                    nameof(ondePerSegmento));

            if (passiPerSegmento < 2)
                throw new ArgumentException(
                    "passiPerSegmento deve essere >= 2.",
                    nameof(passiPerSegmento));


            // ============================================================
            // 1. Eliminiamo eventuali punti consecutivi coincidenti
            // ============================================================

            var p = new List<PointF>();

            p.Add(punti[0]);

            for (int i = 1; i < punti.Length; i++)
            {
                float dx = punti[i].X - p[p.Count - 1].X;
                float dy = punti[i].Y - p[p.Count - 1].Y;

                if (dx * dx + dy * dy > 0.000001f)
                    p.Add(punti[i]);
            }

            if (p.Count < 2)
                return;


            int n = p.Count;


            // ============================================================
            // 2. Lunghezza di ogni segmento
            // ============================================================

            float[] lunghezze = new float[n - 1];

            float lunghezzaTotale = 0f;

            for (int i = 0; i < n - 1; i++)
            {
                float dx = p[i + 1].X - p[i].X;
                float dy = p[i + 1].Y - p[i].Y;

                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                lunghezze[i] = len;
                lunghezzaTotale += len;
            }

            if (lunghezzaTotale < 0.0001f)
                return;

            // ============================================================
            // 3. Versori dei segmenti
            // ============================================================

            float[] ux = new float[n - 1];
            float[] uy = new float[n - 1];

            for (int i = 0; i < n - 1; i++)
            {
                ux[i] =
                    (p[i + 1].X - p[i].X) /
                    lunghezze[i];

                uy[i] =
                    (p[i + 1].Y - p[i].Y) /
                    lunghezze[i];
            }


            // ============================================================
            // 5. Numero totale di campioni
            // ============================================================

            int numeroCampioni =
                (n - 1) * passiPerSegmento;

            if (numeroCampioni < 2)
                numeroCampioni = 2;


            var curva = new List<PointF>(
                numeroCampioni + 1);


            // ============================================================
            // 6. Ogni segmento ha la propria fase: la sinusoide parte e
            //    termina sul vertice, oscillando intorno al segmento rettilineo.
            // ============================================================


            // ============================================================
            // 7. Generazione della sinusoide
            // ============================================================

            for (int s = 0; s < n - 1; s++)
            {
                int numeroPassi = passiPerSegmento;

                for (int i = 0; i <= numeroPassi; i++)
                {
                    // Evita di duplicare il punto di giunzione
                    if (s > 0 && i == 0)
                        continue;


                    float t =
                        i / (float)numeroPassi;


                    // ----------------------------------------------------
                    // Punto sulla linea di base del segmento
                    // ----------------------------------------------------

                    float x =
                        p[s].X +
                        (p[s + 1].X - p[s].X) * t;

                    float y =
                        p[s].Y +
                        (p[s + 1].Y - p[s].Y) * t;


                    // ----------------------------------------------------
                    // Fase locale al segmento: con un numero intero di onde
                    // offset è zero su entrambi i vertici.
                    // ----------------------------------------------------

                    float fase =
                        2.0f *
                        (float)Math.PI *
                        ondePerSegmento *
                        t;


                    float offset =
                        ampiezza * lunghezze[s] *
                        (float)Math.Sin(fase);


                    // ----------------------------------------------------
                    // Normale al segmento rettilineo di base
                    // ----------------------------------------------------

                    float normalX = -uy[s];
                    float normalY = ux[s];


                    // ----------------------------------------------------
                    // Applichiamo lo spostamento sinusoidale
                    // ----------------------------------------------------

                    x += normalX * offset;
                    y += normalY * offset;


                    curva.Add(new PointF(x, y));
                }

            }


            if (curva.Count < 2)
                return;


            // ============================================================
            // 8. Inserimento nel GraphicsPath
            // ============================================================

            path.AddCurve(
                curva.ToArray(),
                0.5f);


            // ============================================================
            // 9. Chiusura opzionale
            // ============================================================

            if (chiudi)
                path.CloseFigure();
        }

        public static void AddSinuousCurve(this GraphicsPath path, GraphicsPath baseline, float ampiezzaRelativa = 0.035f, int numeroOnde = 4, int campioni = 128)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (baseline == null) throw new ArgumentNullException(nameof(baseline));
            if (ampiezzaRelativa < 0f) throw new ArgumentOutOfRangeException(nameof(ampiezzaRelativa));
            if (numeroOnde < 1) throw new ArgumentOutOfRangeException(nameof(numeroOnde));
            if (campioni < 2) throw new ArgumentOutOfRangeException(nameof(campioni));

            using (var flattened = (GraphicsPath)baseline.Clone())
            {
                flattened.Flatten(new Matrix(), 0.5f);
                var rawPoints = flattened.PathPoints;
                var cleanPoints = new List<PointF>();
                foreach (var point in rawPoints)
                {
                    if (cleanPoints.Count == 0)
                        cleanPoints.Add(point);
                    else
                    {
                        float dx = point.X - cleanPoints[cleanPoints.Count - 1].X;
                        float dy = point.Y - cleanPoints[cleanPoints.Count - 1].Y;
                        if (dx * dx + dy * dy > 0.000001f)
                            cleanPoints.Add(point);
                    }
                }
                var points = cleanPoints.ToArray();
                if (points.Length < 2) return;

                var lengths = new float[points.Length - 1];
                var cumulative = new float[points.Length];
                for (int i = 0; i < lengths.Length; i++)
                {
                    float dx = points[i + 1].X - points[i].X;
                    float dy = points[i + 1].Y - points[i].Y;
                    lengths[i] = (float)Math.Sqrt(dx * dx + dy * dy);
                    cumulative[i + 1] = cumulative[i] + lengths[i];
                }

                float totalLength = cumulative[cumulative.Length - 1];
                if (totalLength < 0.0001f) return;
                float amplitude = totalLength * ampiezzaRelativa;
                var wavePoints = new PointF[campioni + 1];
                int segment = 0;
                for (int i = 0; i <= campioni; i++)
                {
                    float distance = totalLength * i / campioni;
                    while (segment < lengths.Length - 1 && cumulative[segment + 1] < distance)
                        segment++;
                    if (lengths[segment] < 0.0001f) continue;

                    float t = (distance - cumulative[segment]) / lengths[segment];
                    t = Math.Max(0f, Math.Min(1f, t));
                    float x = points[segment].X + (points[segment + 1].X - points[segment].X) * t;
                    float y = points[segment].Y + (points[segment + 1].Y - points[segment].Y) * t;
                    float dx = (points[segment + 1].X - points[segment].X) / lengths[segment];
                    float dy = (points[segment + 1].Y - points[segment].Y) / lengths[segment];
                    float offset = amplitude * (float)Math.Sin(2f * (float)Math.PI * numeroOnde * distance / totalLength);
                    wavePoints[i] = new PointF(x - dy * offset, y + dx * offset);
                }

                path.Reset();
                path.AddCurve(wavePoints, 0.5f);
            }
        }



  

 
 
  

        #region SEMICERCHI
        public static void AddSemicerchioSopra(this GraphicsPath path, RectangleF rect)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            DrawSemicerchio(path, rect.X, rect.Y, rect.Width, rect.Height, false);
        }
        public static void AddSemicerchioSopra(this GraphicsPath path, float X0, float Y0, float larghezza, float altezza)
        {
            DrawSemicerchio(path, X0, Y0, larghezza, altezza, false);

        }

        public static void AddSemicerchioSotto(this GraphicsPath path, RectangleF rect)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            DrawSemicerchio(path, rect.X, rect.Y, rect.Width, rect.Height, true);
        }
        public static void AddSemicerchioSotto(this GraphicsPath path, float X0, float Y0, float larghezza, float altezza)
        {
            DrawSemicerchio(path, X0, Y0, larghezza, altezza, true);
        }

        private static void DrawSemicerchio(this GraphicsPath path, float X0, float Y0, float larghezza, float altezza, bool disegnaSotto)
        {
            PointF startPoint = new PointF(X0, Y0);
            PointF currentPoint = new PointF(X0 + larghezza, Y0 + altezza);

            // Calcola la larghezza e altezza disponibili nell'interfaccia (il trascinamento va verso il basso/destra)
            float disponibileWidth = currentPoint.X - startPoint.X;
            float disponibileHeight = disponibileWidth / 2;

            if (disponibileWidth <= 0 || disponibileHeight <= 0)
                return;

            //  Calcolo dei punti chiave riscaldati sullo schermo (estesi verso il basso)
            PointF centroCanestro = new PointF(disponibileWidth / 2, startPoint.Y);
            float raggio = disponibileHeight;
            float angoloInizio = 180f;
            float angoloAmpiezza = 180f;
            if (disegnaSotto)
            {
                angoloInizio = 0f;
                angoloAmpiezza = 180f;
            }

            // rettangolo che contiene il cerchio
            RectangleF boundingBoxCerchio = new RectangleF(startPoint.X, startPoint.Y - raggio, disponibileWidth, disponibileWidth);

            //disegna l'arco
            path.AddArc(boundingBoxCerchio, angoloInizio, angoloAmpiezza);

            path.CloseFigure();
        }
        #endregion SEMICHERCHI

        //@@@???
        public static RectangleF GetContentBounds(this SvgDocument doc)
        {
            if (doc == null) return RectangleF.Empty;

            var result = RectangleF.Empty;
            bool first = true;

            foreach (var el in doc.Descendants())
            {
                GraphicsPath p = null;
                switch (el)
                {
                    case SvgPath sp:
                        //p = sp.Path; break;
                        // Costruisci il GraphicsPath dai segmenti
                        p = new GraphicsPath();
                        if (sp.PathData != null)
                        {
                            // Svg.NET espone SvgPath.PathData come lista di segmenti
                            // ma il modo più semplice è usare i bounds dell'elemento
                            // (vedi alternativa sopra), oppure:
                            // p = sp.PathData.GetPath(); // se la tua versione lo espone
                        }
                        break;
                    case SvgRectangle r:
                        p = new GraphicsPath();
                        p.AddRectangle(new RectangleF(r.X, r.Y, r.Width, r.Height));
                        break;
                    case SvgCircle c:
                        p = new GraphicsPath();
                        p.AddEllipse(c.CenterX - c.Radius, c.CenterY - c.Radius,
                                     c.Radius * 2, c.Radius * 2);
                        break;
                    case SvgEllipse e:
                        p = new GraphicsPath();
                        p.AddEllipse(e.CenterX - e.RadiusX, e.CenterY - e.RadiusY,
                                     e.RadiusX * 2, e.RadiusY * 2);
                        break;
                }
                if (p == null) continue;
                var b = p.GetBounds();
                if (b.Width <= 0 || b.Height <= 0) continue;
                result = first ? b : RectangleF.Union(result, b);
                first = false;
            }
            return result;
        }
        

        public static void AddSVG(this GraphicsPath path, RectangleF rect)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            DrawSemicerchio(path, rect.X, rect.Y, rect.Width, rect.Height, false);
        }

        /// <summary>
        /// Estrae tutti i GraphicsPath dagli elementi vettoriali dell'SVG.
        /// Le trasformazioni cumulative dei nodi vengono applicate ai path.
        /// </summary>
        public static List<GraphicsPath> ExtractPaths(this SvgDocument document)
        {
            var result = new List<GraphicsPath>();
            if (document == null) return result;

            foreach (var element in document.Descendants())
            {
                var path = element.ToSvgPath();
                if (path == null) continue;

                // Applica la trasformazione accumulata risalendo i genitori
                var matrix = element.GetAccumulatedTransform();
                if (matrix != null)
                {
                    path.Transform(matrix);
                    matrix.Dispose();
                }

                result.Add(path);
            }

            return result;
        }

        // Sostituisci questo blocco nel metodo ToGraphicsPath
        public static GraphicsPath ToSvgPath(this SvgElement element)
        {
            if (element == null) return null;

            switch (element)
            {
                case SvgPath p:
                    var pathObj = p.Path(null);
                    return pathObj != null ? (GraphicsPath)pathObj.Clone() : null;

                case SvgRectangle r:
                    {
                        var path = new GraphicsPath();
                        path.AddRectangle(new RectangleF(r.X, r.Y, r.Width, r.Height));
                        return path;
                    }

                case SvgCircle c:
                    {
                        var path = new GraphicsPath();
                        path.AddEllipse(c.CenterX - c.Radius,
                                        c.CenterY - c.Radius,
                                        c.Radius * 2f,
                                        c.Radius * 2f);
                        return path;
                    }

                case SvgEllipse e:
                    {
                        var path = new GraphicsPath();
                        path.AddEllipse(e.CenterX - e.RadiusX,
                                        e.CenterY - e.RadiusY,
                                        e.RadiusX * 2f,
                                        e.RadiusY * 2f);
                        return path;
                    }

                case SvgPolygon poly:
                    {
                        // SvgPolygon.Points è una SvgPointCollection, che contiene una lista piatta di SvgUnit (X0, Y0, X1, Y1, ...)
                        // Quindi bisogna prendere le coppie di valori per creare i PointF
                        var pts = new List<PointF>();
                        var arr = poly.Points.ToArray();
                        for (int i = 0; i + 1 < arr.Length; i += 2)
                        {
                            pts.Add(new PointF(arr[i].Value, arr[i + 1].Value));
                        }
                        if (pts.Count < 3) return null;
                        var path = new GraphicsPath();
                        path.AddPolygon(pts.ToArray());
                        return path;
                    }

                // RIMUOVI questo case duplicato:
                // case SvgPolyline line:

                // SOSTITUISCI con un controllo separato per SvgPolyline
                default:
                    if (element is SvgPolyline line)
                    {
                        // SvgPolyline.Points è una SvgPointCollection, lista piatta di SvgUnit (X0, Y0, X1, Y1, ...)
                        var arr = line.Points.ToArray();
                        var pts = new List<PointF>();
                        for (int i = 0; i + 1 < arr.Length; i += 2)
                        {
                            pts.Add(new PointF(arr[i].Value, arr[i + 1].Value));
                        }
                        if (pts.Count < 2) return null;
                        var path = new GraphicsPath();
                        path.AddLines(pts.ToArray());
                        return path;
                    }
                    else if (element is SvgLine l)
                    {
                        var path = new GraphicsPath();
                        path.AddLine(
                            (float)l.StartX,
                            (float)l.StartY,
                            (float)l.EndX,
                            (float)l.EndY
                        );
                        return path;
                    }
                    return null;
            }
        }

        /// <summary>
        /// Costruisce la matrice di trasformazione cumulata risalendo i genitori.
        /// </summary>
        public static Matrix GetAccumulatedTransform(this SvgElement element)
        {
            var matrix = new Matrix();
            var stack = new Stack<Matrix>();
            var current = element;

            // Raccogli le trasformazioni dal nodo verso la radice
            while (current != null)
            {
                if (current.Transforms != null && current.Transforms.Count > 0)
                {
                    var m = current.Transforms.GetMatrix();
                    if (m != null) stack.Push(m);
                }
                current = current.Parent;
            }

            // Applica dalla radice verso il nodo
            while (stack.Count > 0)
                matrix.Multiply(stack.Pop(), MatrixOrder.Prepend);

            return matrix;
        }

        /// <summary>
        /// Restituisce il bounds complessivo di una lista di GraphicsPath.
        /// </summary>
        public static RectangleF GetUnionBounds(this IEnumerable<GraphicsPath> paths)
        {
            if (paths == null) return RectangleF.Empty;

            RectangleF result = RectangleF.Empty;
            bool first = true;

            foreach (var p in paths)
            {
                if (p == null) continue;
                var b = p.GetBounds();
                if (b.IsEmpty) continue;

                result = first ? b : RectangleF.Union(result, b);
                first = false;
            }

            return result;
        }

    }
 
}

