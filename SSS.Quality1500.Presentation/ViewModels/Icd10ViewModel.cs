namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SSS.Quality1500.Business.Commands.AddIcd10Code;
using SSS.Quality1500.Business.Commands.RemoveIcd10Code;
using SSS.Quality1500.Business.Queries.SearchIcd10Codes;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;
using System.Collections.ObjectModel;

public partial class Icd10ViewModel : ObservableObject
{
    private readonly ICommandHandler<AddIcd10CodeCommand, Result<bool, string>> _addHandler;
    private readonly ICommandHandler<RemoveIcd10CodeCommand, Result<bool, string>> _removeHandler;
    private readonly IQueryHandler<SearchIcd10CodesQuery, Result<Icd10SearchResult, string>> _searchHandler;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _newCode = string.Empty;
    [ObservableProperty] private string _newDescription = string.Empty;
    [ObservableProperty] private int _totalCodes;
    [ObservableProperty] private int _displayedCount;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isConfirmDialogOpen;
    [ObservableProperty] private string _codeToRemove = string.Empty;

    public ObservableCollection<Icd10CodeEntry> Codes { get; } = [];
    public SnackbarMessageQueue MessageQueue { get; } = new(TimeSpan.FromSeconds(3));

    public Icd10ViewModel(
        ICommandHandler<AddIcd10CodeCommand, Result<bool, string>> addHandler,
        ICommandHandler<RemoveIcd10CodeCommand, Result<bool, string>> removeHandler,
        IQueryHandler<SearchIcd10CodesQuery, Result<Icd10SearchResult, string>> searchHandler)
    {
        _addHandler = addHandler;
        _removeHandler = removeHandler;
        _searchHandler = searchHandler;

        _ = SearchAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;

        Result<Icd10SearchResult, string> result = await _searchHandler.HandleAsync(
            new SearchIcd10CodesQuery(SearchText));

        result
            .OnSuccess(searchResult =>
            {
                Codes.Clear();
                foreach (Icd10CodeEntry entry in searchResult.Entries)
                    Codes.Add(entry);

                TotalCodes = searchResult.TotalCount;
                DisplayedCount = searchResult.Entries.Count;
            })
            .OnFailure(error => MessageQueue.Enqueue(error));

        IsLoading = false;
    }

    [RelayCommand]
    private async Task AddCodeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCode) || string.IsNullOrWhiteSpace(NewDescription))
        {
            MessageQueue.Enqueue("Codigo y descripcion son requeridos.");
            return;
        }

        Result<bool, string> result = await _addHandler.HandleAsync(
            new AddIcd10CodeCommand(NewCode, NewDescription));

        if (result.IsFailure)
        {
            MessageQueue.Enqueue(result.GetErrorOrDefault()!);
            return;
        }

        string addedCode = NewCode;
        MessageQueue.Enqueue($"Codigo {addedCode} agregado exitosamente.");
        NewCode = string.Empty;
        NewDescription = string.Empty;
        SearchText = addedCode;
    }

    [RelayCommand]
    private void RequestRemoveCode(string code)
    {
        CodeToRemove = code;
        IsConfirmDialogOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmRemoveAsync()
    {
        IsConfirmDialogOpen = false;
        string code = CodeToRemove;

        Result<bool, string> result = await _removeHandler.HandleAsync(
            new RemoveIcd10CodeCommand(code));

        if (result.IsFailure)
        {
            MessageQueue.Enqueue(result.GetErrorOrDefault()!);
            return;
        }

        MessageQueue.Enqueue($"Codigo {code} eliminado.");
        await SearchAsync();
    }

    [RelayCommand]
    private void CancelRemove()
    {
        IsConfirmDialogOpen = false;
        CodeToRemove = string.Empty;
    }
}
