using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DynamicData;
using KlawiaturaGAvalonia.Models;
using OxyPlot;
using OxyPlot.Avalonia;

namespace KlawiaturaGAvalonia.ViewModels;

public class FitnessWindowViewModel : ViewModelBase
{
    public string PrimaryColour { get; set; } = "White";
    
    private IList<DataPoint> bestFitPoints = new List<DataPoint>();
    private IList<DataPoint> avgFitPoints = new List<DataPoint>();

    public PlotModel PlotModel { get; private set; }
    public OxyPlot.Series.LineSeries BestFitLineSeries { get; private set; }
    public OxyPlot.Series.LineSeries AvgFitLineSeries { get; private set; }
    public FitnessWindowViewModel(List<Summary> inputData)
    {
        PlotModel = new PlotModel { Title = "Fitness plot, Best vs Avg" };
        BestFitLineSeries = new OxyPlot.Series.LineSeries();
        AvgFitLineSeries = new OxyPlot.Series.LineSeries();
        PlotModel.Series.Add(BestFitLineSeries);
        PlotModel.Series.Add(AvgFitLineSeries);
        foreach (var s in inputData)
        {
            bestFitPoints.Add(new DataPoint(s.IdPokolenia,s.BestFitness));
            avgFitPoints.Add(new DataPoint(s.IdPokolenia,s.AvgFitness));
        }
        lock (bestFitPoints)
        {
            BestFitLineSeries.Points.Clear();
            BestFitLineSeries.Points.AddRange(bestFitPoints);
        }
        lock (avgFitPoints)
        {
            AvgFitLineSeries.Points.Clear();
            AvgFitLineSeries.Points.AddRange(avgFitPoints);
        }
        PlotModel.InvalidatePlot(true);
    }
    public FitnessWindowViewModel()
    {
        PlotModel = new PlotModel { Title = "Empty plot constructor" };
        BestFitLineSeries = new OxyPlot.Series.LineSeries();
        AvgFitLineSeries = new OxyPlot.Series.LineSeries();
        bestFitPoints.Add(new DataPoint(0,0));
        avgFitPoints.Add(new DataPoint(0,0));
        PlotModel.Series.Add(BestFitLineSeries);
    }
}