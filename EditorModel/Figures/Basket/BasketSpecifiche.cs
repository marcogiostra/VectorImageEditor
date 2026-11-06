using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorModel.Figures.Basket
{
 
    public static class BasketSpecifiche
    {
        #region LABELS
        public static string Campo = "Campo";
        public static string CampoAttacco = "Campo";
        public static string TrePuntiAttacco = "TrePunti";
        public static string AreaAttacco = "Area";
        public static string AreaPiccolaAttacco = "AreaPiccola";
        public static string CerchioTiroLiberoFuoriAttacco = "CerchioTiroLiberoFuori";
        public static string CerchioTiroLiberoDentroAttacco = "CerchioTiroLiberoDentro";
        public static string LineaSfondoAttacco = "LineaSfondo";
        public static string CanestroAttacco = "Canestro";
        public static string CerchioMetaCampoAttacco = "CerchioMetaCampo";
        public static string CerchioPiccoloMetaCampoAttacco = "CerchioPiccoloMetaCampo";
        public static string ReboundTaccheAttacco = "Tacche";
        public static string ReboundTaccheInterneAttacco = "TaccheInterne";
        //
        public static string CampoDifesa = "Campo";
        public static string TrePuntiDifesa = "TrePunti";
        public static string AreaDifesa = "Area";
        public static string CerchioTiroLiberoFuoriDifesa = "CerchioTiroLiberoFuori";
        public static string CerchioTiroLiberoDentroDifesa = "CerchioTiroLiberoDentro";
        public static string LineaSfondoDifesa = "LineaSfondo";
        public static string CanestroDifesa = "Canestro";
        public static string CerchioMetaCampoDifesa = "CerchioMetaCampo";
        public static string CerchioPiccoloMetaCampoDifesa = "CerchioPiccoloMetaCampo";
        public static string ReboundTaccheDifesa = "Tacche";
        public static string ReboundTaccheInterneDifesa = "TaccheInterne";
        //
        public static string Marks = "Marks";
        public static string LineaRimessa = "LineaRimessa";
        public static string LineaPanchine = "LineaPanchine";
        public static string ZonaCambi = "ZonaCambi";
        //
        public static string Palazzetto = "Palazzetto";


        #endregion LABELS

        #region MISURE_CAMPI_RICALCOLATI
        public static readonly SpecificheCampo FIBA = new SpecificheCampo
        {
            FullLength = 1124,
            FullWidth = 604,
            FullLengthDentro = 600,
            FullWidthDentro = 1120,
            MetaLengh = 563,

            SpessoreLineeCampo = 2,
            SpessoreLineee3Pts = 2,

            CentroCanestroX = 302,
            CentroCanestroY = 65,
            CanestroRaggio = 9,

            Raggio3Punti = 270,
            Distanza3PtsLato = 36,
            SegmentoVerticale3Pts = 119,
            x0_3pts = 38,
            y0_3pts = 0,
            x1_3pts = 566,
            y1_3pts = 335,

            AreaLarghezza = 196,
            AreaLunghezza = 232,
            Areax0 = 204,
            Areay0 = 0,
            Areax1 = 400,
            Areay1 = 234,

            LunettaRaggio = 72,
            x0_Lunetta = 230,
            y0_Lunetta = 234,
            x1_Lunetta = 374,
            y1_Lunetta = 306,

            CerchioCentroRaggio = 72,
            CerchioCentro_xC = 302,
            CerchioCentro_yC = 563,
            CerchioCentro_x0 = 230,
            CerchioCentro_y0 = 563,
            CerchioCentro_x1 = 374,
            CerchioCentro_y1 = 635,

            TabelloneLarghezza = 72,
            Tabellone_x0 = 266,
            Tabellone_y0 = 50,
            Tabellone_x1 = 338,
            Tabellone_y1 = 50,

            NoSfondoRaggio = 50,
            NoSfondo_xC = 302,
            NoSfondo_yC = 65,
            NoSfondo_x0 = 252,
            NoSfondo_y0 = 65,
            NoSfondo_x1 = 352,
            NoSfondo_y1 = 115,

            ReboundTacche = new int[] { 72, 108, 124, 158, 194 },
            Tacche_x0 = 204,
            Tacche_x1 = 400,

        };
        public static readonly SpecificheCampo NBA = new SpecificheCampo
        {
            FullLength = 1150,
            FullWidth = 613,
            FullLengthDentro = 1146,
            FullWidthDentro = 609,
            MetaLengh = 576,

            SpessoreLineeCampo = 2,
            SpessoreLineee3Pts = 2,

            CentroCanestroX = 306,
            CentroCanestroY = 68,
            CanestroRaggio = 9,

            Raggio3Punti = 289,
            Distanza3PtsLato = 38,
            SegmentoVerticale3Pts = 204,
            x0_3pts = 38,
            y0_3pts = 0,
            x1_3pts = 575,
            y1_3pts = 355,

            AreaLarghezza = 231,
            AreaLunghezza = 195,
            Areax0 = 209,
            Areay0 = 0,
            Areax1 = 404,
            Areay1 = 233,

            LunettaRaggio = 73,
            x0_Lunetta = 233,
            y0_Lunetta = 233,
            x1_Lunetta = 575,
            y1_Lunetta = 306,

            CerchioCentroRaggio = 73,
            CerchioCentro_xC = 306,
            CerchioCentro_yC = 574 + 2,
            CerchioCentro_x0 = 233,
            CerchioCentro_y0 = 574 + 2,
            CerchioCentro_x1 = 379,
            CerchioCentro_y1 = 647 + 2,

            TabelloneLarghezza = 73,
            Tabellone_x0 = 270,
            Tabellone_y0 = 50,
            Tabellone_x1 = 343,
            Tabellone_y1 = 50,

            NoSfondoRaggio = 48,
            NoSfondo_xC = 306,
            NoSfondo_yC = 66,
            NoSfondo_x0 = 258,
            NoSfondo_y0 = 66,
            NoSfondo_x1 = 355,
            NoSfondo_y1 = 114,

  



        };
        public static SpecificheCampo WNBA = NBA;
 
        public static readonly SpecificheCampo NCAA_M = new SpecificheCampo
        {
            FullLength = 1150,
            FullWidth = 613,
            FullLengthDentro = 1146,
            FullWidthDentro = 609,
            MetaLengh = 576,

            SpessoreLineeCampo = 2,
            SpessoreLineee3Pts = 2,

            CentroCanestroX = 306,
            CentroCanestroY = 68,
            CanestroRaggio = 9,

            Raggio3Punti = 270,
            Distanza3PtsLato = 42,
            SegmentoVerticale3Pts = 120,
            x0_3pts = 42,
            y0_3pts = 0,
            x1_3pts = 570,
            y1_3pts = 336,

 
            LunettaRaggio = 73,
            x0_Lunetta = 233,
            y0_Lunetta = 233,
            x1_Lunetta = 575,
            y1_Lunetta = 306,

            CerchioCentroRaggio = 73,
            CerchioCentro_xC = 306,
            CerchioCentro_yC = 574 + 2,
            CerchioCentro_x0 = 233,
            CerchioCentro_y0 = 574 + 2,
            CerchioCentro_x1 = 379,
            CerchioCentro_y1 = 647 + 2,


            TabelloneLarghezza = 73,
            Tabellone_x0 = 270,
            Tabellone_y0 = 50,
            Tabellone_x1 = 343,
            Tabellone_y1 = 50,

            NoSfondoRaggio = 48,
            NoSfondo_xC = 306,
            NoSfondo_yC = 66,
            NoSfondo_x0 = 258,
            NoSfondo_y0 = 66,
            NoSfondo_x1 = 355,
            NoSfondo_y1 = 114,

            AreaLarghezza = 170,
            AreaLunghezza = 231,
            Areax0 = 233,
            Areay0 = 0,
            Areax1 = 379,
            Areay1 = 233,



        };
        public static SpecificheCampo NCAA_W = NCAA_M;



        #endregion MISURE_CAMPI_RICALCOLATI

        #region MISURE_CAMPI_ORIGINALI
        public static readonly SpecificheCampoF FIBA_ORIG = new SpecificheCampoF
        {
            FullLength = 2810f,
            FullWidth = 1510f,
            FullLengthDentro = 2800f,
            FullWidthDentro = 1500f,
            MetaLengh = 1407.5f,

            SpessoreLineeCampo = 5f,
            SpessoreLineee3Pts = 5f,

            CentroCanestroX = 755f,
            CentroCanestroY = 162.5f,
            CanestroRaggio = 22.5f,

            Raggio3Punti = 675f,
            Distanza3PtsLato = 95f,
            SegmentoVerticale3Pts = 299f,
            x0_3pts = 95f,
            y0_3pts = 0f,
            x1_3pts = 1415f,
            y1_3pts = 837.5f,

            AreaLarghezza = 490f,
            AreaLunghezza = 580f,
            Areax0 = 510f,
            Areay0 = 0f,
            Areax1 = 1000f,
            Areay1 = 585f,

            LunettaRaggio = 180f,
            x0_Lunetta = 575f,
            y0_Lunetta = 585f,
            x1_Lunetta = 935f,
            y1_Lunetta = 765f,

            CerchioCentroRaggio = 180f,
            CerchioCentro_xC = 755f,
            CerchioCentro_yC = 1407.5f,
            CerchioCentro_x0 = 575f,
            CerchioCentro_y0 = 1407.5f,
            CerchioCentro_x1 = 935f,
            CerchioCentro_y1 = 1587.5f,

            TabelloneLarghezza = 180f,
            Tabellone_x0 = 695f,
            Tabellone_y0 = 125f,
            Tabellone_x1 = 845f,
            Tabellone_y1 = 125f,

            NoSfondoRaggio = 125f,
            NoSfondo_xC = 755f,
            NoSfondo_yC = 125f,
            NoSfondo_x0 = 630f,
            NoSfondo_y0 = 125f,
            NoSfondo_x1 = 880f,
            NoSfondo_y1 = 250f,

            ReboundTacche = new float[] { 180f, 270f, 310f, 395f, 485f },
            Tacche_x0 = 510f,
            Tacche_x1 = 1000f,

            //linea panchine (ESTERNE)
            LineaPanchineSoloAttacco = new[]
            {
                (new PointF(0f,200f), new PointF(200f, 200f)),
                (new PointF(0f,1100f), new PointF(200f, 1100f)) ,
            },
            LineaPanchineSoloDifesa = new[]
            {
                (new PointF(1910f,2302.5f ), new PointF(1710f,2302.5f )),
                (new PointF(1910f,1402.5f), new PointF(1710f,1402.5f )),
            },              
            LineaPanchineFull = new[]          
            {
                (new PointF(0f,200f), new PointF(200f, 200f)),
                (new PointF(0f,1100f), new PointF(200f, 1100f)) ,
                (new PointF(0f,2115f), new PointF(200f, 2115f)) ,
                (new PointF(0f,3015f), new PointF(200f, 3015f)),
            },

            //liene rimesse (ESTERNE)
       
            LineaRimesseEsterneSoloAttacco = new[]
            {
                (new PointF(200f,1037.5f), new PointF(185f, 1037.5f)),
                (new PointF(1710,1037.5f), new PointF(1727f, 1037.5f)),

            },
            LineaRimesseEsterneSoloDifesa = new[]
            {
                (new PointF(1710f,2237.5f), new PointF(1725f,2237.5f )),
                (new PointF(200f,2237.5f), new PointF(185f,2237.5f )),
            },
            LineaRimesseEsterneFull = new[]
            {
                (new PointF(200f,1037.5f), new PointF(185f, 1037.5f)),
                (new PointF(1710,1037.5f), new PointF(1727f, 1037.5f)),
                (new PointF(1710f,2182.5f), new PointF(1725f,2182.5f)),
                (new PointF(200f,2182.5f), new PointF(185f, 2182.5f)),
            },
        };
        public static readonly SpecificheCampoF NBA_ORIG = new SpecificheCampoF
        {
            FullLength = 2875.12f,
            FullWidth = 1534f,
            FullLengthDentro = 2865.12f,
            FullWidthDentro = 1524f,
            MetaLengh = 1436.92f,

            SpessoreLineeCampo = 5.08f,
            SpessoreLineee3Pts = 5.08f,

            CentroCanestroX = 767f,
            CentroCanestroY = 170.02f,

            CanestroRaggio = 22.5f,

            Raggio3Punti = 723.9f,
            Distanza3PtsLato = 96.44f,
            SegmentoVerticale3Pts = 510.54f,
            x0_3pts = 96.44f,
            y0_3pts = 0f,
            x1_3pts = 1437.56f,
            y1_3pts = 888.92f,

            AreaLarghezza = 487.88f,
            AreaLunghezza = 579.12f,
            Areax0 = 523.06f,
            Areay0 = 0f,
            Areax1 = 1010.94f,
            Areay1 = 584.12f,

            LunettaRaggio = 182.88f,
            x0_Lunetta = 593.96f,
            y0_Lunetta = 584.12f,
            x1_Lunetta = 1438f,
            y1_Lunetta = 767f,

            CerchioCentroRaggio = 182.88f,
            CerchioCentro_xC = 767f,
            CerchioCentro_yC = 1436.92f,
            CerchioCentro_x0 = 584.12f,
            CerchioCentro_y0 = 1436.92f,
            CerchioCentro_x1 = 949.88f,
            CerchioCentro_y1 = 1619.8f,

            TabelloneLarghezza = 182.88f,
            Tabellone_x0 = 676F,
            Tabellone_y0 = 126.92f,
            Tabellone_x1 = 858f,
            Tabellone_y1 = 126.92f,

            NoSfondoRaggio = 121.92f,
            NoSfondo_xC = 767f,
            NoSfondo_yC = 126.92f,
            NoSfondo_x0 = 645.08f,
            NoSfondo_y0 = 126.92f,
            NoSfondo_x1 = 888.92f,
            NoSfondo_y1 = 248.84f,
     
            ReboundTacche = new float[] { 218.36f, 253.84f, 345.28f, 436.72f },
            Tacche_x0 = 523.06f,
            Tacche_x1 = 1010.94f,

            //non esistono in NBA, ma servono per la derivazione
            HasMarks = new[]
            {
                (new PointF(431.62f,0f), new PointF(431.62f, 20.24f)),
                (new PointF(1102.4f,0f), new PointF(1102f, 20.24f))
            },

            LineaRimesseInterneSoloAttacco = new[]
            {
                (new PointF(0f,858.44f), new PointF(96.44f, 858.44f)),
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f))
            },
            LineaRimesseInterneSoloDifesa = new[]
            {
                (new PointF(0f,858.44f), new PointF(96.44f, 858.44f)),
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f))
            },
            LineaRimesseInterneFull = new[]
            {
                (new PointF(0f,858.44f), new PointF(96.44f, 858.44f)),
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f))
            },

            //linea panchine (ESTERNE)
            LineaPanchineSoloAttacco = new[]
            {
                (new PointF(108.5f,1058.4f), new PointF(200f, 1058.4f)) ,
            },
            LineaPanchineSoloDifesa = new[]
            {
                 (new PointF(1734f,2295.96f), new PointF(1825.4f,2295.96f)),

            },
            LineaPanchineFull = new[]
            {
                (new PointF(108.5f,1058.4f), new PointF(200f, 1058.4f)) ,
                (new PointF(108.5f,2221.7f), new PointF(200f, 2221.7f)) ,
            },

            ZonaCambiSoloAttacco = new[]
            {
                 (new PointF(189.84f ,1515f), new PointF(200f,1515f)),
            },
            ZonaCambiSoloDifesa = new[]
            {
                 (new PointF(1734f,3132.56f), new PointF(1744.16f,3132.56f)),
            },
            ZonaCambiFull = new[]
            {
                 (new PointF(189.84f,1515f), new PointF(200f,1515f)),
                 (new PointF(189.84f,1762.84f), new PointF(200f,1762.84f)),
            },



        };
        public static SpecificheCampoF WNBA_ORIG = NBA_ORIG;    
        public static readonly SpecificheCampoF NCAA_M_ORIG = new SpecificheCampoF
        {
            FullLength = 2875.12f,
            FullWidth = 1534f,
            FullLengthDentro = 2865.12f,
            FullWidthDentro = 1524f,
            MetaLengh = 1436.92f,

            SpessoreLineeCampo = 5.08f,
            SpessoreLineee3Pts = 5.08f,

            CentroCanestroX = 767f,
            CentroCanestroY = 170.02f,
            CanestroRaggio = 22.5f,

            Raggio3Punti = 675f,
            Distanza3PtsLato = 106.6f,
            SegmentoVerticale3Pts = 301.63f,
            x0_3pts = 106.6f,
            y0_3pts = 0f,
            x1_3pts = 1427.4f,
            y1_3pts = 840.02f,

            LunettaRaggio = 182.88f,
            x0_Lunetta = 593.96f,
            y0_Lunetta = 584.12f,
            x1_Lunetta = 1438f,
            y1_Lunetta = 165.2f,

            CerchioCentroRaggio = 182.88f,
            CerchioCentro_xC = 767f,
            CerchioCentro_yC = 1436.92f,
            CerchioCentro_x0 = 584.12f,
            CerchioCentro_y0 = 1436.92f,
            CerchioCentro_x1 = 949.88f,
            CerchioCentro_y1 = 1619.8f,

            TabelloneLarghezza = 182.88f,
            Tabellone_x0 = 676F,
            Tabellone_y0 = 126.92f,
            Tabellone_x1 = 858f,
            Tabellone_y1 = 126.92f,

            NoSfondoRaggio = 121.92f,
            NoSfondo_xC = 767f,
            NoSfondo_yC = 126.92f,
            NoSfondo_x0 = 645.08f,
            NoSfondo_y0 = 126.92f,
            NoSfondo_x1 = 888.92f,
            NoSfondo_y1 = 248.84f,

            AreaLarghezza = 426.92f,
            AreaLunghezza = 579.12f,
            Areax0 = 584.02f,
            Areay0 = 0f,
            Areax1 = 949.78f,
            Areay1 = 584.12f,


            ReboundTacche = new float[] { 218.36f, 253.84f, 345.28f, 436.72f, 533.16f },
            Tacche_x0 = 584.12f,
            Tacche_x1 = 949.88f - 1f,

            LineaRimesseInterneSoloAttacco = new[]
            {
                (new PointF(0f,858.44f), new PointF(96.44f, 858.44f)),
            },
            LineaRimesseInterneSoloDifesa = new[]
            {
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f))
            },
            LineaRimesseInterneFull = new[]
            {
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f)),
            },
            LineaRimesseInterneFullPalazzetto = new[]
            {
                (new PointF(200f,2277.7f), new PointF(296.44f, 2277.7f)),
            },

            

            //linea panchine (ESTERNE)
            LineaPanchineSoloAttacco = new[]
            {
                (new PointF(108.5f,1058.4f), new PointF(200f, 1058.4f)) ,
            },
            LineaPanchineSoloDifesa = new[]
            {
                 (new PointF(1734f,2295.96f), new PointF(1825.4f,2295.96f)),

            },
            LineaPanchineFull = new[]
            {
                (new PointF(108.5f,1058.4f), new PointF(200f, 1058.4f)) ,
                (new PointF(108.5f,2221.7f), new PointF(200f, 2221.7f)) ,
            },


            LineaRimesseEsterneSoloAttacco = new[]
            {
                 (new PointF(1745.16f,1005.44f), new PointF(1734f,1005.44f)),
            },
            LineaRimesseEsterneSoloDifesa = new[]
            {
                (new PointF(188.84f,2243.44f), new PointF(200f,2243.44f)),
            },
      
            LineaRimesseEsterneFullPalazzetto = new[]
            {
                (new PointF(1734f,1005.44f), new PointF(1745.16f ,1005.44f)),
                (new PointF(1734f,2271.7f), new PointF(1745.16f , 2271.7f)),
            },



        };
        public static readonly SpecificheCampoF NCAA_W_ORIG = new SpecificheCampoF
        {
            FullLength = 2875.12f,
            FullWidth = 1534f,
            FullLengthDentro = 2865.12f,
            FullWidthDentro = 1524f,
            MetaLengh = 1436.92f,

            SpessoreLineeCampo = 5.08f,
            SpessoreLineee3Pts = 5.08f,

            CentroCanestroX = 767f,
            CentroCanestroY = 170.02f,
            CanestroRaggio = 22.5f,

            Raggio3Punti = 675f,
            Distanza3PtsLato = 106.6f,
            SegmentoVerticale3Pts = 301.63f,
            x0_3pts = 106.6f,
            y0_3pts = 0f,
            x1_3pts = 1427.4f,
            y1_3pts = 840.02f,

            LunettaRaggio = 182.88f,
            x0_Lunetta = 593.96f,
            y0_Lunetta = 584.12f,
            x1_Lunetta = 1438f,
            y1_Lunetta = 165.2f,

            CerchioCentroRaggio = 182.88f,
            CerchioCentro_xC = 767f,
            CerchioCentro_yC = 1436.92f,
            CerchioCentro_x0 = 584.12f,
            CerchioCentro_y0 = 1436.92f,
            CerchioCentro_x1 = 949.88f,
            CerchioCentro_y1 = 1619.8f,

            TabelloneLarghezza = 182.88f,
            Tabellone_x0 = 676F,
            Tabellone_y0 = 126.92f,
            Tabellone_x1 = 858f,
            Tabellone_y1 = 126.92f,

            NoSfondoRaggio = 99.06f,
            NoSfondo_xC = 767f,
            NoSfondo_yC = 126.92f,
            NoSfondo_x0 = 667.94f,
            NoSfondo_y0 = 126.92f,
            NoSfondo_x1 = 866.06f,
            NoSfondo_y1 = 271.7f,

            AreaLarghezza = 426.92f,
            AreaLunghezza = 579.12f,
            Areax0 = 584.02f,
            Areay0 = 0f,
            Areax1 = 949.78f,
            Areay1 = 584.12f,


            ReboundTacche = new float[] { 218.36f, 253.84f, 345.28f, 436.72f, 533.16f },
            Tacche_x0 = 584.12f,
            Tacche_x1 = 949.88f - 1f,

            LineaRimesseInterneSoloAttacco = new[]
            {
                (new PointF(0f,858.44f), new PointF(96.44f, 858.44f)),
            },
            LineaRimesseInterneSoloDifesa = new[]
            {
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f))
            },
            LineaRimesseInterneFull = new[]
            {
                (new PointF(1534f,858.44f), new PointF(1442.6f, 858.44f)),
            },
            LineaRimesseInterneFullPalazzetto = new[]
            {
                (new PointF(200f,2277.7f), new PointF(296.44f, 2277.7f)),
            },



            //linea panchine (ESTERNE)
            LineaPanchineSoloAttacco = new[]
            {
                (new PointF(108.5f,1058.4f), new PointF(200f, 1058.4f)) ,
            },
            LineaPanchineSoloDifesa = new[]
            {
                 (new PointF(1734f,2295.96f), new PointF(1825.4f,2295.96f)),

            },
            LineaPanchineFull = new[]
            {
                (new PointF(108.5f,1058.4f), new PointF(200f, 1058.4f)) ,
                (new PointF(108.5f,2221.7f), new PointF(200f, 2221.7f)) ,
            },


            LineaRimesseEsterneSoloAttacco = new[]
            {
                 (new PointF(1745.16f,1005.44f), new PointF(1734f,1005.44f)),
            },
            LineaRimesseEsterneSoloDifesa = new[]
            {
                (new PointF(188.84f,2243.44f), new PointF(200f,2243.44f)),
            },

            LineaRimesseEsterneFullPalazzetto = new[]
            {
                (new PointF(1734f,1005.44f), new PointF(1745.16f ,1005.44f)),
                (new PointF(1734f,2271.7f), new PointF(1745.16f , 2271.7f)),
            },



        };

        #endregion MISURE_CAMPI_ORIGINALI
    }

    #region STRUCTS
    public struct SpecificheCampo
    {
        public int FullLength;                        //LUNGHEZZA CAMPO CON LINEE LATERALI
        public int FullWidth;                         //LARGHEZZA CAMPO CON LINEE LATERALI
        public int FullLengthDentro;                  //LUNGHEZZA CAMPO DI GIOCO
        public int FullWidthDentro;                   //LARGHEZZA CAMPO DI GIOCO
        public int MetaLengh;

        public int SpessoreLineeCampo;
        public int SpessoreLineee3Pts;

        public int CentroCanestroX;
        public int CentroCanestroY;
        public int CanestroRaggio;

        public int Raggio3Punti;
        public int Distanza3PtsLato;
        public int SegmentoVerticale3Pts;
        public int x0_3pts;
        public int y0_3pts;
        public int x1_3pts;
        public int y1_3pts;

        public int AreaLarghezza;
        public int AreaLunghezza;
        public int Areax0;
        public int Areay0;
        public int Areax1;
        public int Areay1;

        public int LunettaRaggio;
        public int x0_Lunetta;
        public int y0_Lunetta;
        public int x1_Lunetta;
        public int y1_Lunetta;

        public int CerchioCentroRaggio;
        public int CerchioCentro_xC;
        public int CerchioCentro_yC;
        public int CerchioCentro_x0;
        public int CerchioCentro_y0;
        public int CerchioCentro_x1;
        public int CerchioCentro_y1;

        public int TabelloneLarghezza;
        public int Tabellone_x0;
        public int Tabellone_y0;
        public int Tabellone_x1;
        public int Tabellone_y1;

        public int NoSfondoRaggio;
        public int NoSfondo_xC;
        public int NoSfondo_yC;
        public int NoSfondo_x0;
        public int NoSfondo_y0;
        public int NoSfondo_x1;
        public int NoSfondo_y1;


        public int[] ReboundTacche;
        public int Tacche_x0;
        public int Tacche_x1;

        public int[] ReboundTacchInterne;
        public int TaccheInterne_x0;
        public int TaccheInterne_x1;

       

    }
    public struct SpecificheCampoF
    {
        public float FullLength;                        //LUNGHEZZA CAMPO CON LINEE LATERALI
        public float FullWidth;                         //LARGHEZZA CAMPO CON LINEE LATERALI
        public float FullLengthDentro;                  //LUNGHEZZA CAMPO DI GIOCO
        public float FullWidthDentro;                   //LARGHEZZA CAMPO DI GIOCO
        public float MetaLengh;

        public float SpessoreLineeCampo;
        public float SpessoreLineee3Pts;

        public float CentroCanestroX;
        public float CentroCanestroY;
        public float CanestroRaggio;

        public float Raggio3Punti;
        public float Distanza3PtsLato;
        public float SegmentoVerticale3Pts;
        public float x0_3pts;
        public float y0_3pts;
        public float x1_3pts;
        public float y1_3pts;

        public float AreaLarghezza;
        public float AreaLunghezza;
        public float Areax0;
        public float Areay0;
        public float Areax1;
        public float Areay1;

        public float LunettaRaggio;
        public float x0_Lunetta;
        public float y0_Lunetta;
        public float x1_Lunetta;
        public float y1_Lunetta;

        public float CerchioCentroRaggio;
        public float CerchioCentro_xC;
        public float CerchioCentro_yC;
        public float CerchioCentro_x0;
        public float CerchioCentro_y0;
        public float CerchioCentro_x1;
        public float CerchioCentro_y1;       

        public float TabelloneLarghezza;
        public float Tabellone_x0;
        public float Tabellone_y0;
        public float Tabellone_x1;
        public float Tabellone_y1;

        public float NoSfondoRaggio;
        public float NoSfondo_xC;
        public float NoSfondo_yC;
        public float NoSfondo_x0;
        public float NoSfondo_y0;
        public float NoSfondo_x1;
        public float NoSfondo_y1;

        public float[] ReboundTacche;
        public float Tacche_x0;
        public float Tacche_x1;

        public (PointF p1, PointF p2)[] HasMarks;
        
        public (PointF p1, PointF p2)[] LineaRimesseEsterneSoloAttacco;
        public (PointF p1, PointF p2)[] LineaRimesseEsterneSoloDifesa;
        public (PointF p1, PointF p2)[] LineaRimesseEsterneFull;
        public (PointF p1, PointF p2)[] LineaRimesseEsterneFullPalazzetto;

        public (PointF p1, PointF p2)[] LineaRimesseInterneSoloAttacco;
        public (PointF p1, PointF p2)[] LineaRimesseInterneSoloDifesa;
        public (PointF p1, PointF p2)[] LineaRimesseInterneFull;
        public (PointF p1, PointF p2)[] LineaRimesseInterneFullPalazzetto;

        public (PointF p1, PointF p2)[] LineaPanchineFull;
        public (PointF p1, PointF p2)[] LineaPanchineSoloAttacco;
        public (PointF p1, PointF p2)[] LineaPanchineSoloDifesa;

        public (PointF p1, PointF p2)[] ZonaCambiSoloAttacco;
        public (PointF p1, PointF p2)[] ZonaCambiSoloDifesa;
        public (PointF p1, PointF p2)[] ZonaCambiFull;




    }
    #endregion STRUCTS


}


