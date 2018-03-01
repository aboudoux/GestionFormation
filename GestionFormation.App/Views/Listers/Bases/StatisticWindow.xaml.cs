using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.PivotGrid;

namespace GestionFormation.App.Views.Listers.Bases
{
    /// <summary>
    /// Logique d'interaction pour StatisticWindow.xaml
    /// </summary>
    public partial class StatisticWindow
    {
        private static string QuantityFieldName = "Quantité";

        public StatisticWindow()
        {
            InitializeComponent();
        }

        private void PivotGrid_OnDataSourceChanged(object sender, RoutedEventArgs e)
        {
            pivotGrid.RetrieveFields();

            foreach (var field in pivotGrid.Fields)
            {
                field.Visible = false;
                field.Caption = field.ToString();
            }

            var dateTimeFields = pivotGrid.Fields.Where(f => f.DataType == typeof(DateTime)).ToList();
            foreach (var field in dateTimeFields)
            {
                AddGroupInterval(field);
            }

            var quantityField = new PivotGridField
            {
                Name = QuantityFieldName,
                UnboundType = FieldUnboundColumnType.Integer,
                Caption = QuantityFieldName,
                Area = FieldArea.DataArea
            };
            pivotGrid.Fields.Add(quantityField);
        }

        private void PivotGrid_OnCustomUnboundFieldData(object sender, PivotCustomFieldDataEventArgs e)
        {
            if (e.Field != null && e.Field.Name == QuantityFieldName)
            {
                e.Value = 1;
            }
        }

        private void CbChart_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (cbChart.SelectedIndex < 0)
                return;
            chartControl.Diagram = ChartFactory.GenerateDiagram((Type) ((ComboBoxEditItem) cbChart.SelectedItem).Tag,ceChart.IsChecked);
            pivotGrid.ChartProvideEmptyCells = IsProvideEmptyCells();
        }

        private void CeChart_OnChecked(object sender, RoutedEventArgs e)
        {

            chartControl.Diagram.SeriesTemplate.LabelsVisibility = object.Equals(ceChart.IsChecked, true);
            chartControl.CrosshairEnabled = Equals(ceChart.IsChecked, false);
        }

        private void CrChartDataVerticalSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            pivotGrid.ChartProvideDataByColumns = crChartDataVertical.SelectedIndex == 1;
        }

        private void ChartControl_OnBoundDataChanged(object sender, RoutedEventArgs e)
        {
            if (chartControl.Diagram is SimpleDiagram2D)
                ConfigurePie();
            if (chartControl.Diagram is SimpleDiagram3D)
                ConfigurePie();
        }

        private void AddGroupInterval(PivotGridField field)
        {
            if (field.DataType != typeof(DateTime))
            {
                throw new Exception(
                    "Impossible d'ajouter un GroupInterval sur un champ qui n'est pas de type 'DateTime'");
            }

            var group = new PivotGridGroup {Caption = field.Caption};
            pivotGrid.Groups.Add(group);

            var fieldYear = new PivotGridField(field.FieldName, field.Area)
            {
                Group = group,
                GroupInterval = FieldGroupInterval.DateYear,
                AllowedAreas = FieldAllowedAreas.All,
                Caption = "Année",
                Visible = false
            };
            var fieldMonth = new PivotGridField(field.FieldName, field.Area)
            {
                Group = group,
                GroupInterval = FieldGroupInterval.DateMonth,
                AllowedAreas = FieldAllowedAreas.All,
                Caption = "Mois",
                Visible = false
            };
            var fieldDay = new PivotGridField(field.FieldName, field.Area)
            {
                Group = group,
                GroupInterval = FieldGroupInterval.DateDay,
                AllowedAreas = FieldAllowedAreas.All,
                Caption = "Jour",
                Visible = false
            };
            var fieldHour = new PivotGridField(field.FieldName, field.Area)
            {
                Group = group,
                GroupInterval = FieldGroupInterval.DateDay,
                AllowedAreas = FieldAllowedAreas.All,
                Caption = "Heure",
                Visible = false
            };

            pivotGrid.Fields.Add(fieldYear);
            pivotGrid.Fields.Add(fieldMonth);
            pivotGrid.Fields.Add(fieldDay);
            pivotGrid.Fields.Add(fieldHour);

            pivotGrid.Fields.Remove(field);
        }

        private void ConfigurePie()
        {
            Dictionary<PieSeries, int> counts = new Dictionary<PieSeries, int>();
            foreach (PieSeries series in chartControl.Diagram.Series)
            {
                counts.Add(series, GetPointsCount(series));
                series.Titles.Add(new Title()
                {
                    Content = series.DisplayName,
                    Dock = Dock.Bottom,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    FontSize = 12,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                });
                series.ShowInLegend = false;
            }

            int max = 0;
            PieSeries maxSeries = null;
            foreach (KeyValuePair<PieSeries, int> pair in counts)
                if (max < pair.Value)
                {
                    max = pair.Value;
                    maxSeries = pair.Key;
                }

            if (maxSeries == null)
                return;
            List<string> values = new List<string>();
            foreach (SeriesPoint point in maxSeries.Points)
                values.Add(point.Argument);

            maxSeries.ShowInLegend = true;

            if (chartControl.Diagram is SimpleDiagram2D)
                foreach (PieSeries series in chartControl.Diagram.Series)
                {
                    foreach (SeriesPoint point in maxSeries.Points)
                        if (!values.Contains(point.Argument))
                        {
                            series.ShowInLegend = true;
                            values.Add(point.Argument);
                        }
                }
        }

        private int GetPointsCount(PieSeries series)
        {
            int count = 0;
            for (int i = 0; i < series.Points.Count; i++)
                if (!double.IsNaN(series.Points[i].Value))
                    count++;
            return count;
        }

        private bool IsProvideEmptyCells()
        {
            if ((chartControl.Diagram is SimpleDiagram2D)
                || (chartControl.Diagram is SimpleDiagram3D)
            )
                return true;
            return false;
        }

        internal static class ChartFactory
        {
            static readonly Type XYDiagram2DType = typeof(XYDiagram2D);
            static readonly Type XYDiagram3DType = typeof(XYDiagram3D);
            static readonly Type SimpleDiagram3DType = typeof(SimpleDiagram3D);
            static readonly Type SimpleDiagram2DType = typeof(SimpleDiagram2D);
            static readonly Type DefaultSeriesType = typeof(BarStackedSeries2D);

            static Dictionary<Type, SeriesTypeDescriptor> seriesTypes;

            public static Dictionary<Type, SeriesTypeDescriptor> SeriesTypes
            {
                get
                {
                    if (seriesTypes == null)
                        seriesTypes = CreateSeriesTypes();
                    return seriesTypes;
                }
            }

            static Dictionary<Type, SeriesTypeDescriptor> CreateSeriesTypes()
            {
                Dictionary<Type, SeriesTypeDescriptor> seriesTypes = new Dictionary<Type, SeriesTypeDescriptor>();
                seriesTypes.Add(typeof(AreaFullStackedSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Aires empilées 100%"});
                seriesTypes.Add(typeof(AreaSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Aires"});
                seriesTypes.Add(typeof(AreaStackedSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Aires empilées"});
                seriesTypes.Add(typeof(BarFullStackedSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Histogramme empilé 100%"});
                seriesTypes.Add(typeof(BarStackedSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Histogramme empilé"});
                seriesTypes.Add(typeof(LineSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Ligne"});
                seriesTypes.Add(typeof(PointSeries2D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram2DType, DisplayText = "Nuage de points"});
                seriesTypes.Add(typeof(AreaSeries3D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram3DType, DisplayText = "Aires 3D"});
                seriesTypes.Add(typeof(AreaStackedSeries3D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram3DType, DisplayText = "Aires empilées 3D"});
                seriesTypes.Add(typeof(AreaFullStackedSeries3D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram3DType, DisplayText = "Aires empilées 100% 3D"});
                seriesTypes.Add(typeof(BarSeries3D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram3DType, DisplayText = "Histogramme 3D"});
                seriesTypes.Add(typeof(PointSeries3D),
                    new SeriesTypeDescriptor {DiagramType = XYDiagram3DType, DisplayText = "Nuage de points 3D"});
                seriesTypes.Add(typeof(PieSeries3D),
                    new SeriesTypeDescriptor {DiagramType = SimpleDiagram3DType, DisplayText = "Anneaux 3D"});
                seriesTypes.Add(typeof(PieSeries2D),
                    new SeriesTypeDescriptor {DiagramType = SimpleDiagram2DType, DisplayText = "Anneaux"});
                return seriesTypes;
            }

            public class SeriesTypeDescriptor
            {
                public Type DiagramType { get; set; }
                public string DisplayText { get; set; }
            }

            public static int CompareComboItemsByStringContent(ComboBoxEditItem first, ComboBoxEditItem second)
            {
                string firstStr = first.Content as string;
                return firstStr == null ? -1 : firstStr.CompareTo(second.Content as string);
            }

            public static void InitComboBox(ComboBoxEdit comboBox, Type[] diagramFilter)
            {
                List<ComboBoxEditItem> itemsList = new List<ComboBoxEditItem>();
                ComboBoxEditItem item, selectedItem = null;
                foreach (Type seriesType in SeriesTypes.Keys)
                {
                    SeriesTypeDescriptor sd = SeriesTypes[seriesType];
                    if (diagramFilter == null || Array.IndexOf(diagramFilter, sd.DiagramType) >= 0)
                    {
                        item = new ComboBoxEditItem();
                        item.Content = sd.DisplayText;
                        item.Tag = seriesType;
                        itemsList.Add(item);
                        if (seriesType == DefaultSeriesType)
                            selectedItem = item;
                    }
                }

                itemsList.Sort(CompareComboItemsByStringContent);
                comboBox.Items.AddRange(itemsList.ToArray());
                comboBox.SelectedItem = selectedItem;
            }

            public static Diagram GenerateDiagram(Type seriesType, bool? showPointsLabels)
            {
                Series seriesTemplate = CreateSeriesInstance(seriesType);
                Diagram diagram = CreateDiagramBySeriesType(seriesType);
                if (diagram is XYDiagram2D)
                    PrepareXYDiagram2D(diagram as XYDiagram2D);
                if (diagram is XYDiagram3D)
                    PrepareXYDiagram3D(diagram as XYDiagram3D);
                if (diagram is Diagram3D)
                    ((Diagram3D) diagram).RuntimeRotation = true;
                diagram.SeriesDataMember = "Series";
                seriesTemplate.ArgumentDataMember = "Arguments";
                seriesTemplate.ValueDataMember = "Values";
                if (seriesTemplate.Label == null)
                    seriesTemplate.Label = new SeriesLabel();
                seriesTemplate.LabelsVisibility = showPointsLabels == true;
                if (!(seriesTemplate is PieSeries2D || seriesTemplate is PieSeries3D))
                {
                    seriesTemplate.ShowInLegend = true;
                }

                diagram.SeriesTemplate = seriesTemplate;
                return diagram;
            }

            static void PrepareXYDiagram2D(XYDiagram2D diagram)
            {
                if (diagram == null) return;
                diagram.AxisX = new AxisX2D();
                diagram.AxisX.Label = new AxisLabel();
                diagram.AxisX.Label.Staggered = true;
            }

            static void PrepareXYDiagram3D(XYDiagram3D diagram)
            {
                if (diagram == null) return;
                diagram.AxisX = new AxisX3D();
                diagram.AxisX.Label = new AxisLabel();
                diagram.AxisX.Label.Visible = false;
            }

            public static Series CreateSeriesInstance(Type seriesType)
            {
                Series series = (Series) Activator.CreateInstance(seriesType);
                ISupportTransparency supportTransparency = series as ISupportTransparency;
                if (supportTransparency != null)
                {
                    bool flag = series is AreaSeries2D;
                    flag = flag || series is AreaSeries3D;
                    if (flag)
                        supportTransparency.Transparency = 0.4;
                    else
                        supportTransparency.Transparency = 0;
                }

                return series;
            }

            static Diagram CreateDiagramBySeriesType(Type seriesType)
            {
                return (Diagram) Activator.CreateInstance(SeriesTypes[seriesType].DiagramType);
            }
        }

        private void StatisticWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ChartFactory.InitComboBox(cbChart, null);

        }
    }
}
