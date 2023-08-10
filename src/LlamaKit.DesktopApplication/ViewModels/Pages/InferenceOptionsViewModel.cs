using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LlamaCpp.Net.Configuration;

namespace LlamaKit.DesktopApplication.ViewModels.Pages;

public partial class InferenceOptionsViewModel : PageViewModel
{
    public InferenceOptionsViewModel()
    {
        this.PenalizeNewLine = true;
        this.MaxNumberOfTokens = 3000;
        this.RepetitionLookback = 100;
        this.SamplingMethod = SamplingMethod.MirostatV2;
        this.Temperature = 0.7f;

        this.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(this.SamplingMethod))
            {
                this.isDefaultSamplingMethod = this.SamplingMethod == SamplingMethod.Default;
            }
        };
    }


    [ObservableProperty] private bool _penalizeNewLine;
    [ObservableProperty] private int _maxNumberOfTokens;
    [ObservableProperty] private int _repetitionLookback;
    [ObservableProperty] private SamplingMethod _samplingMethod;
    [ObservableProperty] private float _temperature;
    public List<SamplingMethod> SamplingMethods => Enum.GetValues<SamplingMethod>().ToList();

    [ObservableProperty] private bool isDefaultSamplingMethod;

    public InferenceOptions ToInferenceOptions()
    {
        return new InferenceOptions()
        {
            PenalizeNewLine = this.PenalizeNewLine,
            MaxNumberOfTokens = this.MaxNumberOfTokens,
            RepetitionLookback = this.RepetitionLookback,
            SamplingMethod = this.SamplingMethod,
            Temperature = this.Temperature
        };
    }
}