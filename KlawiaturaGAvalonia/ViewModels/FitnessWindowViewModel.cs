using System.Collections.Generic;
using System.Linq;
using DynamicData;
using KlawiaturaGAvalonia.Models;
using OxyPlot;
using OxyPlot.Axes;

namespace KlawiaturaGAvalonia.ViewModels;

public class FitnessWindowViewModel : ViewModelBase
{
    public string PrimaryColour { get; set; } = "White";
    
    private IList<DataPoint> _bestFitPoints = new List<DataPoint>();
    private IList<DataPoint> _avgFitPoints = new List<DataPoint>();

    public PlotModel PlotModel { get; private set; }
    public OxyPlot.Series.LineSeries BestFitLineSeries { get; private set; }
    public OxyPlot.Series.LineSeries AvgFitLineSeries { get; private set; }
    public FitnessWindowViewModel(List<Summary> inputData)
    {
        PlotModel = new PlotModel { Title = "Fitness plot, Best vs Avg" };
        BestFitLineSeries = new OxyPlot.Series.LineSeries();
        AvgFitLineSeries = new OxyPlot.Series.LineSeries();
        foreach (var s in inputData)
        {
            _bestFitPoints.Add(new DataPoint(s.IdPokolenia,s.BestFitness));
            _avgFitPoints.Add(new DataPoint(s.IdPokolenia,s.AvgFitness));
        }

        BestFitLineSeries.Title = "Best Fitness";
        BestFitLineSeries.Points.Clear();
        BestFitLineSeries.Points.AddRange(_bestFitPoints);
        
        AvgFitLineSeries.Title = "Average Fitness";
        AvgFitLineSeries.Points.Clear();
        AvgFitLineSeries.Points.AddRange(_avgFitPoints);
            
        PlotModel.Series.Add(BestFitLineSeries);
        PlotModel.Series.Add(AvgFitLineSeries);
        
        PlotModel.InvalidatePlot(true);
    }
    public FitnessWindowViewModel()
    {
        PlotModel = new PlotModel { Title = "Empty plot constructor" };
        BestFitLineSeries = new OxyPlot.Series.LineSeries();
        AvgFitLineSeries = new OxyPlot.Series.LineSeries();
        _bestFitPoints.Add(new DataPoint(0,0));
        _avgFitPoints.Add(new DataPoint(0,0));
        PlotModel.Series.Add(BestFitLineSeries);
    }
}