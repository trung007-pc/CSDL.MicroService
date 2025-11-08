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
using CSDL7.MasterService.Services.Dtos.Wards;
using CSDL7.MasterService.Permissions;
using CSDL7.MasterService.Services.Dtos.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;




namespace CSDL7.Blazor.Server.Components.Pages
{
    public partial class Wards
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        public DataGrid<WardWithNavigationPropertiesDto> DataGridRef { get; set; }
        private IReadOnlyList<WardWithNavigationPropertiesDto> WardList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateWard { get; set; }
        private bool CanEditWard { get; set; }
        private bool CanDeleteWard { get; set; }
        private WardCreateDto NewWard { get; set; }
        private Validations NewWardValidations { get; set; } = new();
        private WardUpdateDto EditingWard { get; set; }
        private Validations EditingWardValidations { get; set; } = new();
        private Guid EditingWardId { get; set; }
        private Modal CreateWardModal { get; set; } = new();
        private Modal EditWardModal { get; set; } = new();
        private GetWardsInput Filter { get; set; }
        private DataGridEntityActionsColumn<WardWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "ward-create-tab";
        protected string SelectedEditTab = "ward-edit-tab";
        private WardWithNavigationPropertiesDto? SelectedWard;
        private IReadOnlyList<LookupDto<Guid>> DistrictsCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        
        public Wards()
        {
            NewWard = new WardCreateDto();
            EditingWard = new WardUpdateDto();
            Filter = new GetWardsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            WardList = new List<WardWithNavigationPropertiesDto>();
            
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetDistrictCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Wards"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            
            
            Toolbar.AddButton(L["NewWard"], async () =>
            {
                await OpenCreateWardModalAsync();
            }, IconName.Add, requiredPolicyName: MasterServicePermissions.Wards.Create);

            return ValueTask.CompletedTask;
        }
        
        private void ToggleDetails(WardWithNavigationPropertiesDto ward)
        {
            DataGridRef.ToggleDetailRow(ward, true);
        }
        
        private bool RowSelectableHandler( RowSelectableEventArgs<WardWithNavigationPropertiesDto> rowSelectableEventArgs )
            => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteWard;
            
        private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<WardWithNavigationPropertiesDto> detailRowTriggerEventArgs)
        {
            detailRowTriggerEventArgs.Toggleable = false;
            detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
            return true;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateWard = await AuthorizationService
                .IsGrantedAsync(MasterServicePermissions.Wards.Create);
            CanEditWard = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Wards.Edit);
            CanDeleteWard = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Wards.Delete);
                            
                            
        }

        private async Task GetWardsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await WardsAppService.GetListAsync(Filter);
            WardList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetWardsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<WardWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetWardsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateWardModalAsync()
        {
            NewWard = new WardCreateDto{
                
                DistrictId = DistrictsCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "ward-create-tab";
            
            
            await NewWardValidations.ClearAll();
            await CreateWardModal.Show();
        }

        private async Task CloseCreateWardModalAsync()
        {
            NewWard = new WardCreateDto{
                
                DistrictId = DistrictsCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateWardModal.Hide();
        }

        private async Task OpenEditWardModalAsync(WardWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "ward-edit-tab";
            
            
            var ward = await WardsAppService.GetWithNavigationPropertiesAsync(input.Ward.Id);
            
            EditingWardId = ward.Ward.Id;
            EditingWard = ObjectMapper.Map<WardDto, WardUpdateDto>(ward.Ward);
            
            await EditingWardValidations.ClearAll();
            await EditWardModal.Show();
        }

        private async Task DeleteWardAsync(WardWithNavigationPropertiesDto input)
        {
            await WardsAppService.DeleteAsync(input.Ward.Id);
            await GetWardsAsync();
        }

        private async Task CreateWardAsync()
        {
            try
            {
                if (await NewWardValidations.ValidateAll() == false)
                {
                    return;
                }

                await WardsAppService.CreateAsync(NewWard);
                await GetWardsAsync();
                await CloseCreateWardModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditWardModalAsync()
        {
            await EditWardModal.Hide();
        }

        private async Task UpdateWardAsync()
        {
            try
            {
                if (await EditingWardValidations.ValidateAll() == false)
                {
                    return;
                }

                await WardsAppService.UpdateAsync(EditingWardId, EditingWard);
                await GetWardsAsync();
                await EditWardModal.Hide();                
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
        protected virtual async Task OnCodeChangedAsync(string? code)
        {
            Filter.Code = code;
            await SearchAsync();
        }
        protected virtual async Task OnDistrictIdChangedAsync(Guid? districtId)
        {
            Filter.DistrictId = districtId;
            await SearchAsync();
        }
        

        private async Task GetDistrictCollectionLookupAsync(string? newValue = null)
        {
            DistrictsCollection = (await WardsAppService.GetDistrictLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }







    }
}
