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
using CSDL7.MasterService.Services.Dtos.Districts;
using CSDL7.MasterService.Permissions;
using CSDL7.MasterService.Services.Dtos.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;




namespace CSDL7.Blazor.Server.Components.Pages
{
    public partial class Districts
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        public DataGrid<DistrictWithNavigationPropertiesDto> DataGridRef { get; set; }
        private IReadOnlyList<DistrictWithNavigationPropertiesDto> DistrictList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDistrict { get; set; }
        private bool CanEditDistrict { get; set; }
        private bool CanDeleteDistrict { get; set; }
        private DistrictCreateDto NewDistrict { get; set; }
        private Validations NewDistrictValidations { get; set; } = new();
        private DistrictUpdateDto EditingDistrict { get; set; }
        private Validations EditingDistrictValidations { get; set; } = new();
        private Guid EditingDistrictId { get; set; }
        private Modal CreateDistrictModal { get; set; } = new();
        private Modal EditDistrictModal { get; set; } = new();
        private GetDistrictsInput Filter { get; set; }
        private DataGridEntityActionsColumn<DistrictWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "district-create-tab";
        protected string SelectedEditTab = "district-edit-tab";
        private DistrictWithNavigationPropertiesDto? SelectedDistrict;
        private IReadOnlyList<LookupDto<Guid>> ProvincesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        
        public Districts()
        {
            NewDistrict = new DistrictCreateDto();
            EditingDistrict = new DistrictUpdateDto();
            Filter = new GetDistrictsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DistrictList = new List<DistrictWithNavigationPropertiesDto>();
            
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetProvinceCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Districts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            
            
            Toolbar.AddButton(L["NewDistrict"], async () =>
            {
                await OpenCreateDistrictModalAsync();
            }, IconName.Add, requiredPolicyName: MasterServicePermissions.Districts.Create);

            return ValueTask.CompletedTask;
        }
        
        private void ToggleDetails(DistrictWithNavigationPropertiesDto district)
        {
            DataGridRef.ToggleDetailRow(district, true);
        }
        
        private bool RowSelectableHandler( RowSelectableEventArgs<DistrictWithNavigationPropertiesDto> rowSelectableEventArgs )
            => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteDistrict;
            
        private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<DistrictWithNavigationPropertiesDto> detailRowTriggerEventArgs)
        {
            detailRowTriggerEventArgs.Toggleable = false;
            detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
            return true;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDistrict = await AuthorizationService
                .IsGrantedAsync(MasterServicePermissions.Districts.Create);
            CanEditDistrict = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Districts.Edit);
            CanDeleteDistrict = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Districts.Delete);
                            
                            
        }

        private async Task GetDistrictsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DistrictsAppService.GetListAsync(Filter);
            DistrictList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDistrictsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DistrictWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDistrictsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDistrictModalAsync()
        {
            NewDistrict = new DistrictCreateDto{
                
                ProvinceId = ProvincesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "district-create-tab";
            
            
            await NewDistrictValidations.ClearAll();
            await CreateDistrictModal.Show();
        }

        private async Task CloseCreateDistrictModalAsync()
        {
            NewDistrict = new DistrictCreateDto{
                
                ProvinceId = ProvincesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateDistrictModal.Hide();
        }

        private async Task OpenEditDistrictModalAsync(DistrictWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "district-edit-tab";
            
            
            var district = await DistrictsAppService.GetWithNavigationPropertiesAsync(input.District.Id);
            
            EditingDistrictId = district.District.Id;
            EditingDistrict = ObjectMapper.Map<DistrictDto, DistrictUpdateDto>(district.District);
            
            await EditingDistrictValidations.ClearAll();
            await EditDistrictModal.Show();
        }

        private async Task DeleteDistrictAsync(DistrictWithNavigationPropertiesDto input)
        {
            await DistrictsAppService.DeleteAsync(input.District.Id);
            await GetDistrictsAsync();
        }

        private async Task CreateDistrictAsync()
        {
            try
            {
                if (await NewDistrictValidations.ValidateAll() == false)
                {
                    return;
                }

                await DistrictsAppService.CreateAsync(NewDistrict);
                await GetDistrictsAsync();
                await CloseCreateDistrictModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditDistrictModalAsync()
        {
            await EditDistrictModal.Hide();
        }

        private async Task UpdateDistrictAsync()
        {
            try
            {
                if (await EditingDistrictValidations.ValidateAll() == false)
                {
                    return;
                }

                await DistrictsAppService.UpdateAsync(EditingDistrictId, EditingDistrict);
                await GetDistrictsAsync();
                await EditDistrictModal.Hide();                
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
        protected virtual async Task OnProvinceIdChangedAsync(Guid? provinceId)
        {
            Filter.ProvinceId = provinceId;
            await SearchAsync();
        }
        

        private async Task GetProvinceCollectionLookupAsync(string? newValue = null)
        {
            ProvincesCollection = (await DistrictsAppService.GetProvinceLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }







    }
}
