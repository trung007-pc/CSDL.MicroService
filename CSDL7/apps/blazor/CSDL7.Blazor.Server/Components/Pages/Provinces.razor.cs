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
using CSDL7.MasterService.Services.Dtos.Provinces;
using CSDL7.MasterService.Permissions;
using CSDL7.MasterService.Services.Dtos.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;




namespace CSDL7.Blazor.Server.Components.Pages
{
    public partial class Provinces
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        public DataGrid<ProvinceDto> DataGridRef { get; set; }
        private IReadOnlyList<ProvinceDto> ProvinceList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateProvince { get; set; }
        private bool CanEditProvince { get; set; }
        private bool CanDeleteProvince { get; set; }
        private ProvinceCreateDto NewProvince { get; set; }
        private Validations NewProvinceValidations { get; set; } = new();
        private ProvinceUpdateDto EditingProvince { get; set; }
        private Validations EditingProvinceValidations { get; set; } = new();
        private Guid EditingProvinceId { get; set; }
        private Modal CreateProvinceModal { get; set; } = new();
        private Modal EditProvinceModal { get; set; } = new();
        private GetProvincesInput Filter { get; set; }
        private DataGridEntityActionsColumn<ProvinceDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "province-create-tab";
        protected string SelectedEditTab = "province-edit-tab";
        private ProvinceDto? SelectedProvince;
        
        
        
        
        
        
        public Provinces()
        {
            NewProvince = new ProvinceCreateDto();
            EditingProvince = new ProvinceUpdateDto();
            Filter = new GetProvincesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ProvinceList = new List<ProvinceDto>();
            
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Provinces"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            
            
            Toolbar.AddButton(L["NewProvince"], async () =>
            {
                await OpenCreateProvinceModalAsync();
            }, IconName.Add, requiredPolicyName: MasterServicePermissions.Provinces.Create);

            return ValueTask.CompletedTask;
        }
        
        private void ToggleDetails(ProvinceDto province)
        {
            DataGridRef.ToggleDetailRow(province, true);
        }
        
        private bool RowSelectableHandler( RowSelectableEventArgs<ProvinceDto> rowSelectableEventArgs )
            => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteProvince;
            
        private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<ProvinceDto> detailRowTriggerEventArgs)
        {
            detailRowTriggerEventArgs.Toggleable = false;
            detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
            return true;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateProvince = await AuthorizationService
                .IsGrantedAsync(MasterServicePermissions.Provinces.Create);
            CanEditProvince = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Provinces.Edit);
            CanDeleteProvince = await AuthorizationService
                            .IsGrantedAsync(MasterServicePermissions.Provinces.Delete);
                            
                            
        }

        private async Task GetProvincesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ProvincesAppService.GetListAsync(Filter);
            ProvinceList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetProvincesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProvinceDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetProvincesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateProvinceModalAsync()
        {
            NewProvince = new ProvinceCreateDto{
                
                
            };

            SelectedCreateTab = "province-create-tab";
            
            
            await NewProvinceValidations.ClearAll();
            await CreateProvinceModal.Show();
        }

        private async Task CloseCreateProvinceModalAsync()
        {
            NewProvince = new ProvinceCreateDto{
                
                
            };
            await CreateProvinceModal.Hide();
        }

        private async Task OpenEditProvinceModalAsync(ProvinceDto input)
        {
            SelectedEditTab = "province-edit-tab";
            
            
            var province = await ProvincesAppService.GetAsync(input.Id);
            
            EditingProvinceId = province.Id;
            EditingProvince = ObjectMapper.Map<ProvinceDto, ProvinceUpdateDto>(province);
            
            await EditingProvinceValidations.ClearAll();
            await EditProvinceModal.Show();
        }

        private async Task DeleteProvinceAsync(ProvinceDto input)
        {
            await ProvincesAppService.DeleteAsync(input.Id);
            await GetProvincesAsync();
        }

        private async Task CreateProvinceAsync()
        {
            try
            {
                if (await NewProvinceValidations.ValidateAll() == false)
                {
                    return;
                }

                await ProvincesAppService.CreateAsync(NewProvince);
                await GetProvincesAsync();
                await CloseCreateProvinceModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditProvinceModalAsync()
        {
            await EditProvinceModal.Hide();
        }

        private async Task UpdateProvinceAsync()
        {
            try
            {
                if (await EditingProvinceValidations.ValidateAll() == false)
                {
                    return;
                }

                await ProvincesAppService.UpdateAsync(EditingProvinceId, EditingProvince);
                await GetProvincesAsync();
                await EditProvinceModal.Hide();                
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
        protected virtual async Task OnProvinceCodeChangedAsync(string? provinceCode)
        {
            Filter.ProvinceCode = provinceCode;
            await SearchAsync();
        }
        







    }
}
