# MVVM MessageBox Service for WPF (using Dependency Injection)
A C# MVVM Service for displaying simple MessageBox messages with custom styles included - 
Uses MVVM Toolkit with Microsoft.Extensions.DependencyInjection but can be adapted to any MVVM / DI Library.
This service illustrate how you can have a view model that displays dialog boxes but still can be easily unit tested.

In the example the MainViewModel is unit tested at 100% code coverage even with a complex path that display mutiple dialogs with various choices.

Include samples and an Example to unit test code involving a complex path with multiple MessageBoxes

![image](https://user-images.githubusercontent.com/2603287/160229368-ef56548c-b44d-453b-9a7e-9d2dff79dc5e.png)

![image](https://user-images.githubusercontent.com/2603287/160229385-f2cc2f08-2724-432c-85e1-48e430503f73.png)

![image](https://user-images.githubusercontent.com/2603287/160229399-8c15e03b-4cab-4e7e-ac66-69333f773490.png)

![image](https://user-images.githubusercontent.com/2603287/160229418-fa5083a9-979e-4df9-9399-172b241c9665.png)

This shows a function including multiple  decisions triggered by user input.
The Unit test module shows how to unit test the function for full code coverage.

![Complex Path](https://github.com/pierre01/MessageBox/blob/development/MessageBoxPathToTest.jpg)
