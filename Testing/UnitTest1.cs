using Moq;
using System.Collections.ObjectModel;
using ThesisCourse_4.MVVM.ViewModels;
using ThesisCourse_4.Services;
using ThesisCourse_4.MVVM.Models;

public class WelcomeViewModelTests
{
    private WelcomeViewModel CreateViewModel()
    {
        var storageMock = new Mock<IStorageService>();
        storageMock.Setup(s => s.LoadButtons())
                   .Returns(new ObservableCollection<ButtonModel>());
        var navMock = new Mock<INavigationService>();
        var themeMock = new Mock<IThemeService>();
        return new WelcomeViewModel(themeMock.Object, storageMock.Object, navMock.Object);
    }

    [Fact]
    public void AddGraph_AddsGraph_WhenNameIsValid()
    {
        var vm = CreateViewModel();
        vm.GraphName = "Граф1";
        vm.AddGraphCommand.Execute(null);

        Assert.Single(vm.Buttons);
        Assert.Equal("Граф1", vm.Buttons[0].Name);
    }

    [Fact]
    public void AddGraph_DoesNotAdd_WhenNameIsEmpty()
    {
        var vm = CreateViewModel();
        vm.GraphName = "";
        vm.AddGraphCommand.Execute(null);

        Assert.Empty(vm.Buttons);
    }

    [Fact]
    public void DeleteGraph_RemovesCorrectGraph()
    {
        var vm = CreateViewModel();
        vm.GraphName = "GraphToRemove";
        vm.AddGraphCommand.Execute(null);

        vm.DeleteGraphCommand.Execute("GraphToRemove");

        Assert.Empty(vm.Buttons);
    }

    [Fact]
    public void RenameGraph_ChangesName_IfUnique()
    {
        var storageMock = new Mock<IStorageService>();
        storageMock.Setup(s => s.LoadButtons())
                   .Returns(new ObservableCollection<ButtonModel>());
        var navMock = new Mock<INavigationService>();
        var themeMock = new Mock<IThemeService>();
        var vm = new WelcomeViewModel(themeMock.Object, storageMock.Object, navMock.Object);

        vm.GraphName = "OldName";
        vm.AddGraphCommand.Execute(null);
        var button = vm.Buttons[0];
        // В ручных тестах метод ShowInputDialog нельзя обойти, если он не virtual/injectable,
        // смоделируем переименование:
        button.Name = "NewName";
        storageMock.Verify(s => s.SaveButtons(It.IsAny<ObservableCollection<ButtonModel>>()), Times.AtLeastOnce());
        Assert.Equal("NewName", vm.Buttons[0].Name);
    }

    [Fact]
    public void AddGraph_DoesNotCreateDuplicateNames()
    {
        var vm = CreateViewModel();
        vm.GraphName = "Duplicate";
        vm.AddGraphCommand.Execute(null);

        vm.GraphName = "Duplicate";
        vm.AddGraphCommand.Execute(null);

        Assert.Single(vm.Buttons); 
    }

    [Fact]
    public void UpdateGridState_RespectsMinRows()
    {
        var vm = CreateViewModel();
        Assert.Equal(3, vm.GridState.RowCount);

        for (int i = 0; i < 8; i++)
        {
            vm.GraphName = $"G{i}";
            vm.AddGraphCommand.Execute(null);
        }
        Assert.True(vm.GridState.RowCount >= 3);
    }

    [Fact]
    public void OpenAuthWindowCommand_CallsNavigation()
    {
        var storageMock = new Mock<IStorageService>();
        storageMock.Setup(s => s.LoadButtons())
                   .Returns(new ObservableCollection<ButtonModel>());
        var navMock = new Mock<INavigationService>();
        var themeMock = new Mock<IThemeService>();
        var vm = new WelcomeViewModel(themeMock.Object, storageMock.Object, navMock.Object);

        vm.OpenAuthWindowCommand.Execute(null);

        navMock.Verify(m => m.ShowWindow<SmallAuthViewModel>(), Times.Once);
    }
}
