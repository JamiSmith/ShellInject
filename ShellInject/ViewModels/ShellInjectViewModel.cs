﻿using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ShellInject.Interfaces;

namespace ShellInject;

/// The ShellInjectViewModel class is a base class that provides common functionality for view models in the ShellInjectViewModel namespace.
/// It inherits from ObservableObject and implements the IShellInjectShellViewModel interface.
public partial class ShellInjectViewModel : ObservableObject,  IShellInjectShellViewModel
{
    public bool IsInitialized { get; private set; }
    
    bool IShellInjectShellViewModel.IsInitialized
    {
        get => IsInitialized;
        set => IsInitialized = value;
    }
    
    /// when the view associated with the view model appears on the screen.
    public ICommand OnAppearingCommand { get; }

    /// <summary>
    /// Gets or sets the command that is executed when the page is disappearing.
    /// </summary>
    /// <remarks>
    /// This command is executed when the page is about to disappear from the screen.
    /// It can be used to perform any necessary cleanup or finalize any operations before the page is no longer visible.
    /// </remarks>
    public ICommand OnDisAppearingCommand { get; } 
    
    /// <summary>
    /// The ShellInjectViewModel class is a base class that provides common functionality for view models in the ShellInjectViewModel namespace.
    /// It inherits from ObservableObject and implements the IShellInjectViewModel interface.
    /// </summary>
    public ShellInjectViewModel()
    {
        OnAppearingCommand = new Command(OnAppearing);
        OnDisAppearingCommand = new Command(OnDisAppearing);
    }

    /// <summary>
    /// An asynchronous method that can be overridden to handle initialization logic for the view model.
    /// This method is executed when the view model requires initialization, allowing derived classes
    /// to implement their specific setup processes or tasks.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public virtual Task InitializedAsync() => Task.CompletedTask;
    
    /// <summary>
    /// Sends data to the view model asynchronously.
    /// </summary>
    /// <param name="parameter">The data parameter to be sent to the view model.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task DataReceivedAsync(object? parameter) => Task.CompletedTask;

    /// <summary>
    /// Reverses the data received asynchronously.
    /// </summary>
    /// <param name="parameter">The parameter of type object that contains the data to be reversed.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public virtual Task ReverseDataReceivedAsync(object? parameter) => Task.CompletedTask;
    
    /// <summary>
    /// Executes when the view associated with the view model is appearing on the screen.
    /// </summary>
    public virtual void OnAppearing() { }

    /// <summary>
    /// Executes when the view associated with the view model is disappearing from the screen.
    /// </summary>
    public virtual void OnDisAppearing() { }

    /// <summary>
    /// Executes asynchronous actions when the associated view or page has fully appeared in the UI.
    /// This method can be overridden by derived classes to implement custom behavior.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnAppearedAsync()
    {
        return Task.CompletedTask;
    }
}