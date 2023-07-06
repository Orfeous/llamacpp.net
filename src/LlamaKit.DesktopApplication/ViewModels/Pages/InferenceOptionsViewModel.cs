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
    }


    [ObservableProperty] private bool _penalizeNewLine;
    [ObservableProperty] private int _maxNumberOfTokens;
    [ObservableProperty] private int _repetitionLookback;
    [ObservableProperty] private SamplingMethod _samplingMethod;
    public List<SamplingMethod> SamplingMethods => Enum.GetValues<SamplingMethod>().ToList();

    public InferenceOptions ToInferenceOptions()
    {
        return new InferenceOptions()
        {
            PenalizeNewLine = this.PenalizeNewLine,
            MaxNumberOfTokens = this.MaxNumberOfTokens,
            RepetitionLookback = this.RepetitionLookback,
            SamplingMethod = this.SamplingMethod
        };
    }
}