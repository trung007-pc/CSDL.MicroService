using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using CSDL7.MasterService.Services.Dtos.Departments;
using CSDL7.MasterService.Permissions;
using CSDL7.MasterService.Services.Departments;
using CSDL7.MasterService.Services.Dtos.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;




namespace CSDL7.Blazor.Server.Components.Pages
{
    public partial class Departments
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        public DataGrid<DepartmentDto> DataGridRef { get; set; }
        private IReadOnlyList<DepartmentDto> DepartmentList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDepartment { get; set; }
        private bool CanEditDepartment { get; set; }
        private bool CanDeleteDepartment { get; set; }
        private CreateDepartmentDto NewDepartment { get; set; }
        private Validations NewDepartmentValidations { get; set; } = new();
        private UpdateDepartmentDto EditingDepartment { get; set; }
        private Validations EditingDepartmentValidations { get; set; } = new();
        private Guid EditingDepartmentId { get; set; }
        private Modal CreateDepartmentModal { get; set; } = new();
        private Modal EditDepartmentModal { get; set; } = new();
        private GetDepartmentsInput Filter { get; set; }
        private DataGridEntityActionsColumn<DepartmentDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "department-create-tab";
        protected string SelectedEditTab = "department-edit-tab";
        private DepartmentDto? SelectedDepartment;
        
        
        
        
        
        
        public Departments()
        {
            NewDepartment = new CreateDepartmentDto();
            EditingDepartment = new UpdateDepartmentDto();
            Filter = new GetDepartmentsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DepartmentList = new List<DepartmentDto>();
            
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Departments"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            
            
            Toolbar.AddButton(L["NewDepartment"], async () =>
            {
                await OpenCreateDepartmentModalAsync();
            }, IconName.Add, requiredPolicyName: MasterServicePermissions.Departments.Create);
            
            Toolbar.AddButton("Ping Email", async () =>
            { 
                await DepartmentsAppService.CallPingEmailAsync();
            }, IconName.Alert, requiredPolicyName: MasterServicePermissions.Departments.Create);
            return ValueTask.CompletedTask;
        }
        
        private void ToggleDetails(DepartmentDto department)
        {
            DataGridRef.ToggleDetailRow(department, true);
        }
        
        private bool RowSelectableHandler( RowSelectableEventArgs<DepartmentDto> rowSelectableEventArgs )
            => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteDepartment;
            
        private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<DepartmentDto> detailRowTriggerEventArgs)
        {
            detailRowTriggerEventArgs.Toggleable = false;
            detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
            return true;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDepartment = await AuthorizationService
                .IsGrantedAsync(MasterServicePermissions.Departments.Create);
            CanEditDepartment = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Departments.Edit);
            CanDeleteDepartment = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Departments.Delete);
                            
                            
        }

        private async Task GetDepartmentsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DepartmentsAppService.GetListAsync(Filter);
            DepartmentList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDepartmentsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DepartmentDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDepartmentsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDepartmentModalAsync()
        {
            NewDepartment = new CreateDepartmentDto{
                
                
            };

            SelectedCreateTab = "department-create-tab";
            
            
            await NewDepartmentValidations.ClearAll();
            await CreateDepartmentModal.Show();
        }

        private async Task CloseCreateDepartmentModalAsync()
        {
            NewDepartment = new CreateDepartmentDto{
                
                
            };
            await CreateDepartmentModal.Hide();
        }

        private async Task OpenEditDepartmentModalAsync(DepartmentDto input)
        {
            SelectedEditTab = "department-edit-tab";
            
            
            var department = await DepartmentsAppService.GetAsync(input.Id);
            
            EditingDepartmentId = department.Id;
            EditingDepartment = ObjectMapper.Map<DepartmentDto, UpdateDepartmentDto>(department);
            
            await EditingDepartmentValidations.ClearAll();
            await EditDepartmentModal.Show();
        }

        private async Task DeleteDepartmentAsync(DepartmentDto input)
        {
            await DepartmentsAppService.DeleteAsync(input.Id);
            await GetDepartmentsAsync();
        }

        private async Task CreateDepartmentAsync()
        {
            try
            {
                if (await NewDepartmentValidations.ValidateAll() == false)
                {
                    return;
                }

                await DepartmentsAppService.CreateAsync(NewDepartment);
                await GetDepartmentsAsync();
                await CloseCreateDepartmentModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditDepartmentModalAsync()
        {
            await EditDepartmentModal.Hide();
        }

        private async Task UpdateDepartmentAsync()
        {
            try
            {
                if (await EditingDepartmentValidations.ValidateAll() == false)
                {
                    return;
                }

                await DepartmentsAppService.UpdateAsync(EditingDepartmentId, EditingDepartment);
                await GetDepartmentsAsync();
                await EditDepartmentModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }









        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        







    }
}
