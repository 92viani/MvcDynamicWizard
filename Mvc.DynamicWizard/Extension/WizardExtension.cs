using Mvc.DynamicWizard.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Mvc.DynamicWizard.Extension
{
    public static class WizardExtension
    {
        public static MvcHtmlString WizardHeader(this HtmlHelper html, WizardManager wm, bool count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("\r\n");
            TagBuilder tagBuilder = new TagBuilder("h2");
            tagBuilder.InnerHtml = wm.CurrentStep.StepTitle;
            if (count)
            {
                TagBuilder varWB = tagBuilder;
                varWB.InnerHtml = varWB.InnerHtml + "  " + wm.IndexCount;
            }
            tagBuilder.MergeAttribute("class", "wizardtitle");
            stringBuilder.Append(tagBuilder.ToString()).Append("\r\n");
            TagBuilder tagBuilder2 = new TagBuilder("h3");
            tagBuilder2.InnerHtml = wm.CurrentStep.StepSubTitle;
            tagBuilder2.MergeAttribute("class", "wizardsubtitle");
            stringBuilder.Append(tagBuilder2.ToString()).Append("\r\n");
            TagBuilder tagBuilder3 = new TagBuilder("div");
            tagBuilder3.MergeAttribute("class", "wizardheader");
            tagBuilder3.InnerHtml = stringBuilder.ToString();
            return MvcHtmlString.Create(tagBuilder3.ToString());
        }
        public static MvcHtmlString WizardFooter(this HtmlHelper html, WizardManager wm, string next, string back, string confirm)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!wm.CurrentStep.IsFirst)
            {
                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttribute("class", "wizardbutton");
                tagBuilder.MergeAttribute("type", "submit");
                tagBuilder.MergeAttribute("name", "back");
                tagBuilder.MergeAttribute("id", "back");
                tagBuilder.MergeAttribute("value", back);
                stringBuilder.Append(tagBuilder.ToString()).Append("\r\n");
            }
            TagBuilder tagBuilder2 = new TagBuilder("input");
            tagBuilder2.MergeAttribute("class", "wizardbutton");
            tagBuilder2.MergeAttribute("type", "submit");
            tagBuilder2.MergeAttribute("name", "next");
            tagBuilder2.MergeAttribute("id", "next");
            if (wm.CurrentStep.IsLast)
            {
                tagBuilder2.MergeAttribute("value", confirm);
            }
            else
            {
                tagBuilder2.MergeAttribute("value", next);
            }
            stringBuilder.Append(tagBuilder2.ToString()).Append("\r\n");
            TagBuilder tagBuilder3 = new TagBuilder("div");
            tagBuilder3.MergeAttribute("class", "wizardbuttons");
            tagBuilder3.InnerHtml = stringBuilder.ToString();
            return MvcHtmlString.Create(tagBuilder3.ToString());
        }
        public static MvcHtmlString WizardStepBody(this HtmlHelper html, WizardManager wm, object stepModel)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("\r\n");
            PropertyInfo[] properties = stepModel.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                object obj_ = propertyInfo.GetGetMethod().Invoke(stepModel, null);
                if (obj_ == null)
                {
                    obj_ = "";
                }
                TagBuilder tagBuilder = new TagBuilder("div");
                tagBuilder.MergeAttribute("class", "wizardfield");
                if (propertyInfo.PropertyType == typeof(bool))
                {
                    tagBuilder.InnerHtml = propertyInfo.Name + ":  " + InputExtensions.CheckBox(html, propertyInfo.Name, obj_).ToHtmlString();
                }
                else
                {
                    if (propertyInfo.PropertyType.IsSubclassOf(typeof(Enum)))
                    {
                        List<SelectListItem> list = new List<SelectListItem>();
                        IEnumerator enumerator = Enum.GetValues(propertyInfo.PropertyType).GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                Enum @enum = (Enum)enumerator.Current;
                                List<SelectListItem> varJWA0 = list;
                                SelectListItem selectListItem = new SelectListItem();
                                selectListItem.Selected=false;
                                selectListItem.Text=@enum.ToString();
                                selectListItem.Value=@enum.ToString();
                                varJWA0.Add(selectListItem);
                            }
                        }
                        finally
                        {
                            IDisposable disposable_ = enumerator as IDisposable;
                            if (disposable_ != null)
                            {
                                disposable_.Dispose();
                            }
                        }
                        tagBuilder.InnerHtml=propertyInfo.Name + ":  " + SelectExtensions.DropDownList(html, propertyInfo.Name, list).ToHtmlString();
                    }
                    else
                    {
                        tagBuilder.InnerHtml=propertyInfo.Name + ":  " + InputExtensions.TextBox(html, propertyInfo.Name, obj_).ToHtmlString();
                    }
                }
                stringBuilder.Append(tagBuilder.ToString()).Append("\r\n");
            }
            TagBuilder tagBuilder2 = new TagBuilder("div");
            tagBuilder2.MergeAttribute("class", "wizardbody");
            tagBuilder2.InnerHtml=stringBuilder.ToString();
            return MvcHtmlString.Create(tagBuilder2.ToString());
        }
        public static MvcForm BeginWizardFormView(this HtmlHelper html, string controllerName)
        {
            return FormExtensions.BeginForm(html, "ExecuteStep", controllerName);
        }
        public static MvcForm BeginWizardForm(this HtmlHelper html, string controllerName)
        {
            return FormExtensions.BeginForm(html, "ActionStep", controllerName);
        }
        public static MvcForm BeginWizardFormPartial(this HtmlHelper html, string controllerName)
        {
            return FormExtensions.BeginForm(html, "ActionStepPartial", controllerName);
        }
        public static MvcForm BeginAutoWizardForm(this HtmlHelper html, string controllerName)
        {
            return FormExtensions.BeginForm(html, "AutoStep", controllerName);
        }
    }
}
