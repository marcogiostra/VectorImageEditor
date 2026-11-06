using System;
using System.Collections.Generic;
using EditorModel.Common;
using System.Drawing;
using EditorModel.Geometry;
using EditorModel.Renderers;
using EditorModel.Style;
using System.Drawing.Drawing2D;
using Svg;
using EditorModel.Geometry.GeometryBasket;
using EditorModel.Figures.Extentions;

namespace EditorModel.Figures
{
    public static class FigureBuilder
    {
        private const int MARKER_SIZE = 8;

        public static void BuildSquareGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddRectangle(new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^(AllowedOperations.Size | AllowedOperations.Vertex))
            { Name = "Square" };
        }

        public static void BuildRectangleGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddRectangle(new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ AllowedOperations.Vertex)
            { Name = "Rectangle" };
        }

        public static void BuildRoundedRectangleGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            var radius = 0.2f;
            path.Path.AddPath(RoundedRectangle.Create(new RectangleF(-0.5f, -0.5f, 1, 1),
                                                      radius < 0 ? 0 : radius > 0.5f ? 0.5f : radius), false);
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ AllowedOperations.Vertex)
            { Name = "RoundedRectangle" };
        }

        public static void BuildCircleGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddEllipse(new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, 
                AllowedOperations.All ^ (AllowedOperations.Size | AllowedOperations.Rotate | AllowedOperations.Vertex))
            { Name = "Circle" };
        }

        public static void BuildEllipseGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddEllipse(new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ AllowedOperations.Vertex)
            { Name = "Ellipse" };
        }

        public static void BuildMarkerGeometry(Figure marker)
        {
            var path = new SerializableGraphicsPath();
            marker.Style.BorderStyle.Width = 0;
            switch (((Marker)marker).MarkerType)
            {
                case MarkerType.ControlBezier:
                    path.Path.AddEllipse(new RectangleF(-MARKER_SIZE / 2f, -MARKER_SIZE / 2f, MARKER_SIZE, MARKER_SIZE));
                    break;
                default:
                    path.Path.AddRectangle(new RectangleF(-MARKER_SIZE / 2f, -MARKER_SIZE / 2f, MARKER_SIZE, MARKER_SIZE));
                    break;
            }
            marker.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ 
                (AllowedOperations.Size | AllowedOperations.Rotate | AllowedOperations.Select | 
                 AllowedOperations.Skew | AllowedOperations.Vertex | AllowedOperations.Pathed | AllowedOperations.Warp))
            { Name = "Marker" };
        }

        public static void BuildTextGeometry(Figure figure, string text)
        {
            figure.Geometry = new TextGeometry { Text = text, Name = "Text" };
            figure.Style.FillStyle = new DefaultFill(AllowedFillDecorators.All ^
                AllowedFillDecorators.RadialGradient) { Color = Color.Black };
            figure.Style.BorderStyle.IsVisible = false; 
        }

    
        public static void BuildTextRenderGeometry(Figure figure, string text)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddRectangle(new RectangleF(-0.5f, -0.5f, 1, 1));

            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ 
                (AllowedOperations.Vertex | AllowedOperations.Pathed))
            { Name = "TextBlock" };

            figure.Style.BorderStyle = null; // отключение рамки для рендера
            figure.Style.FillStyle = new DefaultFill(AllowedFillDecorators.All ^
                AllowedFillDecorators.RadialGradient) { Color = Color.Black };

            figure.Renderer = new TextRenderer(text);
        }

       
        public static void BuildBezierTextGeometry(Figure figure, string text)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddBezier(-0.5f, -0.5f,  0, 1,  0.5f, -0.5f,  1, 1);

            figure.Geometry = new PrimitiveBezier(path, AllowedOperations.All ^ 
                (AllowedOperations.Pathed | AllowedOperations.Skew))
            { Name = "BezierText" };

            figure.Style.BorderStyle = null; 
            figure.Style.FillStyle = new DefaultFill(AllowedFillDecorators.All ^
                AllowedFillDecorators.RadialGradient)
            { Color = Color.Black };

            figure.Renderer = new BezierTextRenderer(text) { FontSize = 14f };
        }

      
        public static void BuildImageRenderGeometry(Figure figure, Bitmap image)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddRectangle(new RectangleF(-0.5f, -0.5f, 1, 1));

            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^
                (AllowedOperations.Vertex | AllowedOperations.Pathed))
            { Name = "Image" };
            figure.Style.BorderStyle = null; 
            figure.Style.FillStyle = null; 
            figure.Renderer = new ImageRenderer(image);     
        }

        public static void BuildSvgRenderGeometry(Figure figure, SvgDocument svgDocument)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddRectangle(new RectangleF(-0.5f, -0.5f, 1, 1));

            //figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ (AllowedOperations.Vertex | AllowedOperations.Pathed  ))
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.Rotate)
            { Name = "Svg" };
            figure.Style.BorderStyle = null;
            figure.Style.FillStyle = null;
            figure.Renderer = new MioSvgRenderer(svgDocument);
           
        }



        public static void BuildPolyGeometry(Figure figure, bool isClosed = true, PointF[] points = null)
        {
            figure.Style.FillStyle.IsVisible = isClosed;
            figure.Geometry = new PolygoneGeometry(isClosed, points) { Name = isClosed ? "Polygon" : "Polyline" };
            NormalizeVertex(figure);
        }

        public static void BuildPalleggioBasketGeometry(Figure figure, bool isClosed = true, PointF[] points = null)
        {
            BuildBasketPolylineGeometry(figure, BasketPolylineKind.PalleggioSinusoidale, isClosed, points);
        }

        public static void BuildGiocatoreAttaccoRotondoGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddEllipse(new RectangleF(-20f, -20f, 40f, 40f));
            figure.Geometry = new PrimitiveGeometry(path, EditorModel.Geometry.AllowedOperations.Select)
            { Name = "GiocatoreAttaccoRotondo", Title = "GiocatoreAttaccoRotondo" };
            figure.Style.FillStyle.IsVisible = false;
            figure.PlayerNumber = new PlayerNumberAnnotation { Position = PlayerNumberPosition.N, FontScale = 0.45f };
            figure.Renderer = new RoundAttackPlayerRenderer();
        }

        public static void BuildGiocatoreDifesaTriangolareGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            const float halfBase = 23.094011f; // 40 / sqrt(3): triangolo equilatero alto 40
            path.Path.AddPolygon(new[]
            {
                new PointF(0f, -20f),
                new PointF(-halfBase, 20f),
                new PointF(halfBase, 20f)
            });
            figure.Geometry = new PrimitiveGeometry(path, EditorModel.Geometry.AllowedOperations.Select)
            { Name = "GiocatoreDifesaTriangolare", Title = "GiocatoreDifesaTriangolare" };
            figure.Style.FillStyle.IsVisible = false;
            figure.PlayerNumber = new PlayerNumberAnnotation { FontScale = 0.45f };
            figure.Renderer = new RoundAttackPlayerRenderer();
        }

        public static void BuildGiocatoreStilizzatoGeometry(Figure figure)
        {
            figure.Geometry = new StylizedPlayerGeometry();
            figure.Style.FillStyle.Color = Color.White;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.Color = Color.Black;
            figure.Style.BorderStyle.IsVisible = true;
            figure.PlayerNumber = new PlayerNumberAnnotation { Position = PlayerNumberPosition.N, FontScale = 0.45f };
            figure.Renderer = new StylizedPlayerRenderer();
        }

        public static void BuildGiocatoreStilizzatoSempliceGeometry(Figure figure)
        {
            figure.Geometry = new StylizedPlayerSimpleGeometry();
            figure.Style.FillStyle.Color = Color.White;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.Color = Color.Black;
            figure.Style.BorderStyle.IsVisible = true;
            figure.PlayerNumber = new PlayerNumberAnnotation { Position = PlayerNumberPosition.N, FontScale = 0.45f };
            figure.Renderer = new StylizedPlayerSimpleRenderer();
        }

        public static void BuildGiocatoreGeometry(Figure figure)
        {
            figure.Geometry = new GiocatoreGeometry();
            figure.Style.FillStyle.Color = Color.White;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.Color = Color.Black;
            figure.Style.BorderStyle.IsVisible = true;
            figure.PlayerNumber = new PlayerNumberAnnotation { Position = PlayerNumberPosition.N, FontScale = 0.35f };
            figure.UpdateGiocatoreTitleFromPlayerNumber();
            figure.Renderer = new GiocatoreRenderer();
        }

        public static void BuildGiocatoreXGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            // The two diagonal bars overlap at the center; winding fill keeps
            // that intersection solid instead of cancelling it as an even-odd hole.
            path.Path.FillMode = FillMode.Winding;
            path.Path.AddPolygon(new[]
            {
                new PointF(-14f, -18f), new PointF(-8f, -18f), new PointF(14f, 12f),
                new PointF(14f, 18f), new PointF(8f, 18f), new PointF(-14f, -12f)
            });
            path.Path.AddPolygon(new[]
            {
                new PointF(8f, -18f), new PointF(14f, -18f), new PointF(14f, -12f),
                new PointF(-8f, 18f), new PointF(-14f, 18f), new PointF(-14f, 12f)
            });
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.Select | AllowedOperations.Rotate)
            { Name = "GiocatoreX", Title = "GiocatoreX" };
            figure.Style.FillStyle.Color = Color.Black;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.IsVisible = false;
            figure.PlayerNumber = new PlayerNumberAnnotation
            {
                IsVisible = false,
                Position = PlayerNumberPosition.E,
                FontScale = 0.25f
            };
            figure.Renderer = new GiocatoreXRenderer();
        }

        public static void BuildGiocatoreNumeroGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddRectangle(new RectangleF(-17f, -18f, 34f, 36f));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.Select | AllowedOperations.Rotate)
            { Name = "GiocatoreNumero", Title = "GiocatoreNumero0" };
            figure.Style.FillStyle.IsVisible = false;
            figure.Style.BorderStyle.IsVisible = false;
            figure.PlayerNumber = new PlayerNumberAnnotation
            {
                Text = "0",
                IsVisible = true,
                FontScale = 1f,
                Position = PlayerNumberPosition.N
            };
            figure.Renderer = new GiocatoreNumeroRenderer();
        }

        public static void BuildGiocatoreOGeometry(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.FillMode = FillMode.Alternate;
            path.Path.AddEllipse(new RectangleF(-15f, -18f, 30f, 36f));
            path.Path.AddEllipse(new RectangleF(-9f, -12f, 18f, 24f));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.Select | AllowedOperations.Rotate)
            { Name = "GiocatoreO", Title = "GiocatoreO" };
            figure.Style.FillStyle.Color = Color.Black;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.IsVisible = false;
            figure.PlayerNumber = new PlayerNumberAnnotation
            {
                IsVisible = false,
                Position = PlayerNumberPosition.E,
                FontScale = 0.25f
            };
            figure.Renderer = new GiocatoreXRenderer();
        }

        public static void BuildOggettoBasketGeometry(Figure figure)
        {
            figure.Geometry = new OggettoBasketGeometry();
            figure.Style.FillStyle.Color = Color.Orange;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.Color = Color.Black;
            figure.Style.BorderStyle.IsVisible = true;
            figure.Renderer = new OggettoBasketRenderer();
        }

        public static void BuildBallBasketGeometry(Figure figure)
        {
            figure.Geometry = new BallBasketGeometry();
            figure.Style.FillStyle.Color = Color.OrangeRed;
            figure.Style.FillStyle.IsVisible = true;
            figure.Style.BorderStyle.Color = Color.Black;
            figure.Style.BorderStyle.IsVisible = true;
            figure.Renderer = new BallBasketRenderer();
        }

        public static void BuildBasketPolylineGeometry(Figure figure, BasketPolylineKind kind, bool isClosed = false, PointF[] points = null)
        {
            figure.Style.FillStyle.IsVisible = isClosed;
            ConfigureBasketStroke(figure, kind);

            figure.Geometry = new PolygoneGeometry(isClosed, points)
            {
                Name = kind == BasketPolylineKind.PalleggioTratteggiato ? "PassaggioBasket"
                    : kind == BasketPolylineKind.PalleggioSinusoidale ? "PalleggioBasket"
                    : kind == BasketPolylineKind.TiroPuntinato ? "TiroBasket"
                    : kind.ToString() + "Basket",
                IsSinusoidal = kind == BasketPolylineKind.PalleggioSinusoidale,
                BasketKind = kind
            };

            ConfigureBasketRenderer(figure, kind);
            // Preserve drawing coordinates; normalization would rescale a regenerated sine path.
        }

        public static void BuildBasketBezierGeometry(Figure figure, BasketPolylineKind kind, PointF[] points, byte[] types)
        {
            BuildBezierGeometry(figure, points, types, kind);
        }

        private static void ConfigureBasketStroke(Figure figure, BasketPolylineKind kind)
        {
            figure.Style.BorderStyle.DashStyle = kind == BasketPolylineKind.PalleggioTratteggiato
                ? DashStyle.Custom
                : kind == BasketPolylineKind.TiroPuntinato ? DashStyle.Dot : DashStyle.Solid;
            figure.Style.BorderStyle.DashPattern = kind == BasketPolylineKind.PalleggioTratteggiato
                ? new[] { 8f, 8f }
                : null;
            if (kind == BasketPolylineKind.TiroPuntinato)
                figure.Style.BorderStyle.Width = 2f;
        }

        private static void ConfigureBasketRenderer(Figure figure, BasketPolylineKind kind)
        {
            if (kind == BasketPolylineKind.Spostamento ||
                kind == BasketPolylineKind.PalleggioTratteggiato ||
                kind == BasketPolylineKind.TiroPuntinato ||
                kind == BasketPolylineKind.PalleggioSinusoidale)
            {
                figure.Renderer = new ArrowRendererDecorator(figure.Renderer)
                {
                    ArrowPosition = ArrowPosition.End,
                    AlignWithLastGeometrySegment = true
                };
            }
        }

        


        public static void BuildBezierGeometry(Figure figure, PointF[] points, byte[] types, BasketPolylineKind basketKind = BasketPolylineKind.None)
        {
            figure.Style.FillStyle.IsVisible = false;
            if (basketKind != BasketPolylineKind.None)
            {
                ConfigureBasketStroke(figure, basketKind);
                var name = basketKind == BasketPolylineKind.PalleggioTratteggiato ? "PassaggioBasketBezier"
                    : basketKind == BasketPolylineKind.PalleggioSinusoidale ? "PalleggioBasketBezier"
                    : basketKind == BasketPolylineKind.TiroPuntinato ? "TiroBasketBezier"
                    : basketKind.ToString() + "BasketBezier";
                figure.Geometry = new BezierGeometry(points, types) { Name = name, BasketKind = basketKind };
                ConfigureBasketRenderer(figure, basketKind);
            }
            else
                figure.Geometry = new BezierGeometry(points, types) { Name = "Bezier" };
            if (basketKind == BasketPolylineKind.None && types.Length > 3)
                NormalizeVertex(figure);
        }

        private static void NormalizeVertex(Figure figure)
        {
            var transformed = figure.Geometry as ITransformedGeometry;
            if (transformed != null)
            {
                var bounds = figure.GetTransformedPath().Path.GetBounds();
                var points = transformed.GetTransformedPoints(figure);
                figure.Transform.Matrix.TransformPoints(points);
                transformed.SetTransformedPoints(figure, points);
                figure.Transform.Matrix = new Matrix();
                var eps = Helper.EPSILON;
                var kfx = (bounds.Width < eps) ? eps : 1 / bounds.Width;
                var kfy = (bounds.Height < eps) ? eps : 1 / bounds.Height;
                var pts = transformed.GetTransformedPoints(figure);
                for (var i = 0; i < pts.Length; i++)
                {
                    pts[i].X = (bounds.Width < eps) ? 0 : (pts[i].X - bounds.X) / bounds.Width - 0.5f;
                    pts[i].Y = (bounds.Height < eps) ? 0 : (pts[i].Y - bounds.Y) / bounds.Height - 0.5f;
                }
                transformed.SetTransformedPoints(figure, pts);
                var m = new Matrix();
                m.Translate(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                m.Scale(1 / kfx, 1 / kfy);
                figure.Transform.Matrix = m;
            }
        }

     
        public static void BuildWedgeGeometry(Figure figure, WedgeKind kind)
        {
            figure.Style.FillStyle.IsVisible = kind != WedgeKind.Arc;
            figure.Geometry = new WedgeGeometry(kind, 0, 270);
        }

    
        public static void BuildRegularGeometry(Figure figure, int vertexCount)
        {
            if (vertexCount < 3) throw new ArgumentOutOfRangeException("vertexCount",
                "Il numero di vertici deve essere pari o superiore a tre.");

            const float radius = 0.5f;
            var points = new List<PointF>();
            var stepAngle = Math.PI * 2 / vertexCount;
            var angle = - Math.PI * 2;
            for (var i = 0; i < vertexCount; i++)
            {
                points.Add(new PointF((float)(radius * Math.Cos(angle)), (float)(radius * Math.Sin(angle))));
                angle += stepAngle;
            }

            var path = new SerializableGraphicsPath();
            path.Path.AddPolygon(points.ToArray());

            var names = new[] {"Nothing", "Circle", "Line", "Triangle", "Romb", "Pentagon", "Gexagon" };

            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ 
                (AllowedOperations.Vertex | AllowedOperations.Size | AllowedOperations.Skew))
            { Name = vertexCount <= 6 ? names[vertexCount] : "Regular" + vertexCount };
        }

        
        public static void BuildFrameGeometry(Figure figure, Point startPoint)
        {
            figure.Style.BorderStyle.Width = 0;
            figure.Geometry = new FrameGeometry(startPoint) { Name = "Frame" };
        }

        public static void BuildAddLineGeometry(Figure figure, Point startPoint, bool isClosed, bool isSmoothed, bool isSinusoidal = false, BasketPolylineKind basketKind = BasketPolylineKind.None)
        {
            figure.Style.BorderStyle.Width = 0;
            figure.Style.BorderStyle.DashStyle = basketKind == BasketPolylineKind.PalleggioTratteggiato
                ? DashStyle.Custom
                : basketKind == BasketPolylineKind.TiroPuntinato ? DashStyle.Dot : DashStyle.Solid;
            figure.Style.BorderStyle.DashPattern = basketKind == BasketPolylineKind.PalleggioTratteggiato
                ? new[] { 8f, 8f }
                : null;
            figure.Geometry = new AddLineGeometry(startPoint)
            {
                Name = "AddLine",
                IsClosed = isClosed,
                IsSmoothed = isSmoothed,
                IsSinusoidal = isSinusoidal,
                BasketKind = basketKind
            };
        }

        //nuova parte generia
        public static void BuildSemicerchioSopra(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddSemicerchioSopra(new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ AllowedOperations.Vertex)
            { Name = "SemicerchioSopra" };
        }

        public static void BuildSemicerchioSotto(Figure figure)
        {
            var path = new SerializableGraphicsPath();
            path.Path.AddSemicerchioSotto(new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ AllowedOperations.Vertex)
            { Name = "SemicerchioSotto" };
        }

  

        #region BASKET
        public static void BuildOggettoCampoBasketAttaccoGeometry(Figure figure, CourtyCampionato camp,bool IsFullCamp, bool DrawPalazzetto, BasketCourtObjectType oggettoCampoBasket)
        {
            BuildOggettoCampoBasket(figure,camp, IsFullCamp, DrawPalazzetto, oggettoCampoBasket, true);
        }
        public static void BuildOggettoCampoBasketDifesaGeometry(Figure figure, CourtyCampionato camp, bool IsFullCamp, bool DrawPalazzetto,BasketCourtObjectType oggettoCampoBasket)
        {
            BuildOggettoCampoBasket(figure, camp, IsFullCamp, DrawPalazzetto, oggettoCampoBasket, false);
        }

        private static void BuildOggettoCampoBasket(Figure figure, CourtyCampionato camp, bool IsFullCamp, bool DrawPalazzetto, BasketCourtObjectType oggettoCampoBasket, bool IsAttacco)
        {
            string name = string.Empty;

            switch (oggettoCampoBasket)
            {
                case BasketCourtObjectType.CanestroSoloAttacco:
                case BasketCourtObjectType.CanestroSoloDifesa:
                case BasketCourtObjectType.CanestroFullDifesa:
                case BasketCourtObjectType.CanestroFullDifesaPalazzetto:
                    name = "Canestro";
                    break;

                case BasketCourtObjectType.LineaNoSfondoAttacco:
                case BasketCourtObjectType.LineaNoSfondoDifesa:
                case BasketCourtObjectType.LineaNoSfondoDifesaFull:
                case BasketCourtObjectType.LineaNoSfondoDifesaFullPalazzetto:
                    name = "LineaNoSfondo";
                    break;

                case BasketCourtObjectType.TaccheAttacco:
                case BasketCourtObjectType.TaccheDifesa:
                case BasketCourtObjectType.TaccheDifesaFull:
                case BasketCourtObjectType.TaccheDifesaFullPalazzetto:
                    name = "Tacche";
                    break;

                case BasketCourtObjectType.LineeFuoriCampo:
                    name = "LineeFuoriCampo";
                    break;

                case BasketCourtObjectType.HasMarks:
                    name = "HasMarks";
                    break;

                case BasketCourtObjectType.LineaRimesseEsterneAttacco:
                case BasketCourtObjectType.LineaRimesseEsterneDifessa:
                case BasketCourtObjectType.LineaRimesseEsterneFull:
                case BasketCourtObjectType.LineaRimesseEsterneFullPalazzetto:
                    name = "LineaRimesseEsterne";
                    break;

                case BasketCourtObjectType.LineaRimesseInterneSoloAttacco:
                case BasketCourtObjectType.LineaRimesseInterneSoloDifesa:
                case BasketCourtObjectType.LineaRimesseInterneFull:
                case BasketCourtObjectType.LineaRimesseInterneFullPalazzetto:
                    name = "LineaRimesseInterne";
                    break;


                case BasketCourtObjectType.LineaPanchineDifesa:
                case BasketCourtObjectType.LineaPanchineAttacco:
                case BasketCourtObjectType.LineaPanchineFull:
                    name = "LineaPanchine";
                    break;

                case BasketCourtObjectType.ZonaCambiSoloAttacco:
                case BasketCourtObjectType.ZonaCambiSoloDifesa:
                case BasketCourtObjectType.ZonaCambiFull:
                    name = "ZonaCambi";
                    break;

                case BasketCourtObjectType.LineaTiroTrePunti:
                    name = "LineaTiroTrePunti";
                    break;


            }


            var path = new SerializableGraphicsPath();
            if (IsAttacco)
                path.Path.AddOggettoCampoBasketAttacco(camp, IsFullCamp, DrawPalazzetto, oggettoCampoBasket, new RectangleF(-0.5f, -0.5f, 1, 1));
            else
                path.Path.AddOggettoCampoBasketDifesa(camp, IsFullCamp, DrawPalazzetto, oggettoCampoBasket, new RectangleF(-0.5f, -0.5f, 1, 1));
            figure.Geometry = new PrimitiveGeometry(path, AllowedOperations.All ^ AllowedOperations.Vertex)
                { Name = name };
        }
  
        #endregion BASKET

    



	}
}
