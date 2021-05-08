using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using System.Threading.Tasks;

namespace MASA.Blazor
{
    public partial class MSelect<TItem> : BSelect<TItem>
    {
        private BoundingClientRect _rect;

        [Parameter] public bool Dark { get; set; }

        [Parameter] public int MinWidth { get; set; }

        private int Width => _visible || _text.Any() ? ComputeLabelLength() * 6 : 0;
        protected override string LegendStyle => $"width: {Width}px";

        protected override Task OnInitializedAsync()
        {
            _icon = "mdi-menu-down";

            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _rect = await JsInvokeAsync<BoundingClientRect>(JsInteropConstants.GetBoundingClientRect, Ref);
            }
        }

        protected override void SetComponentClass()
        {
            CssProvider
                .AsProvider<BSelect<TItem>>()
                .Apply(cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input m-text-field m-text-field--is-booted m-select")
                        .AddIf("m-input--is-disabled", () => Disabled)
                        .AddIf("m-input--dense", () => Dense)
                        .AddIf("m-text-field--enclosed m-text-field--filled", () => Filled)
                        .AddIf("m-text-field--enclosed m-text-field--outlined", () => Outlined)
                        .AddIf("m-text-field--enclosed m-text-field--single-line m-text-field--solo", () => Solo)
                        .AddIf("m-input--is-focused primary--text", () => _focused)
                        .AddIf("m-select--is-menu-active", () => _visible)
                        .AddTheme(Dark);
                }, styleBuilder =>
                {
                    styleBuilder
                        .AddIf(() => $"min-width:{MinWidth}px", () => MinWidth != 0);
                })
                .Apply("control", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__control");
                })
                .Apply("slot", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__slot");
                })
                .Apply("select-slot", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-select__slot");
                })
                .Apply("label", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-label")
                        .AddIf("m-label--active", () => { return Solo ? false : _visible || _text.Any(); })
                        .AddIf("primary--text", () => { return Solo ? false : _focused; })
                        .AddTheme(Dark);
                }, styleBuilder =>
                {
                    styleBuilder
                        .Add("left: 0px; right: auto; position: absolute");
                })
                .Apply("selector", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-select__selections");
                }, styleBuilder =>
                {
                    styleBuilder
                        .Add("m-select__selection--comma");
                })
                .Apply("select-arrow", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__append-inner");
                })
                .Apply("select-arrow-icon", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__icon m-input__icon--append");
                })
                .Apply("hit", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-text--field__details");
                });

            SlotProvider
                .Apply<BIcon, MIcon>(props =>
                {
                    props[nameof(MIcon.Class)] = _visible ? "primary--text" : "";
                })
                .Apply<BPopover, MPopover>(props =>
                {
                    props[nameof(MPopover.Class)] = "m-menu__content menuable__content__active";
                    props[nameof(MPopover.Visible)] = (_visible && Items != null);
                    props[nameof(MPopover.MinWidth)] = (StringOrNumber)_rect?.Width;
                    props[nameof(MPopover.MaxHeight)] = (StringOrNumber)400;
                })
                .Apply<BOverlay, MOverlay>(props =>
                {
                    props[nameof(MOverlay.Value)] = _visible;
                    props[nameof(MOverlay.Click)] =
                        EventCallback.Factory.Create<MouseEventArgs>(this, () => { _visible = false; });
                    props[nameof(MOverlay.Opacity)] = (StringOrNumber)0;
                })
                .Apply<BList, MList>(props =>
                {
                    props[nameof(MList.Dense)] = Dense;
                })
                .Apply<BListItemGroup, MListItemGroup>(props =>
                {
                    props[nameof(MListItemGroup.Color)] = "primary";

                    if (Multiple)
                    {
                        props[nameof(MListItemGroup.Multiple)] = Multiple;
                        props[nameof(MListItemGroup.Values)] = Values;
                    }
                    else
                    {
                        props[nameof(MListItemGroup.Value)] = Value;
                    }
                })
                .Apply<BSelectOption<TItem>, MSelectOption<TItem>>()
                .Apply<BChip, MChip>()
                .Apply<BHitMessage, MHitMessage>();
        }

        private int ComputeLabelLength()
        {
            if (string.IsNullOrEmpty(Label))
            {
                return 0;
            }

            var length = 0;
            for (int i = 0; i < Label.Length; i++)
            {
                if (Label[i] > 127)
                {
                    length += 2;
                }
                else
                {
                    length += 1;
                }
            }

            return length + 1;
        }
    }
}