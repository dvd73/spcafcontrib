using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using MOSS.Common.Utilities;
using MOSS.Common.Code;

namespace MOSS.Common.Controls
{
    public class BaseListFieldIterator : Microsoft.SharePoint.WebControls.ListFieldIterator
    {
        // create dynamic setter methods that wrap the internal
        // ControlMode and FieldName properties of the TemplateContainer class       
        protected static GenericSetter set_TemplateContainer_ControlMode = ILHelper.CreateSetMethod(typeof(TemplateContainer), "ControlMode");
        protected static GenericSetter set_TemplateContainer_FieldName = ILHelper.CreateSetMethod(typeof(TemplateContainer), "FieldName");

        protected List<FieldDisplayRuleItem> _dynamicRules = new List<FieldDisplayRuleItem>();

        // get references for the frequently used objects
        protected SPFormContext _formContext = SPContext.Current.FormContext;

        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            CheckListFieldIterator();
            InstantiateField();
        }

        protected void InstantiateField()
        {
            for (int i = 0; i < base.Fields.Count; i++)
            {
                SPField field = base.Fields[i];
                String fieldName = field.InternalName;

                // check if the current field is on the list of "hidden" fields in the current display mode
                // or whether there is a "global" rule to hide fields
                FieldDisplayRuleItem exception = _dynamicRules.FirstOrDefault(
                    // empty (null) value means there is no restriction for the control mode
                    e => ((e.ControlModes == null) || (e.ControlModes.Contains(ControlMode))) &&
                        // empty (null) value means there is no restriction for the field name
                    ((e.FieldNames == null) || (e.FieldNames.Contains(fieldName))) &&
                    (e.Rule == FieldDisplayRule.Hidden));

                if ((!this.IsFieldExcluded(field)) && (exception == null))
                {
                    TemplateContainer child = new TemplateContainer();
                    this.Controls.Add(child);
                    SPControlMode controlMode = GetControlMode(fieldName);
                    // use the dynamic setter to access internal properties
                    set_TemplateContainer_ControlMode(child, controlMode);
                    set_TemplateContainer_FieldName(child, fieldName);

                    this.ControlTemplate.InstantiateIn(child);
                }
            }
        }

        protected void CheckListFieldIterator()
        {
            if (this.ControlTemplate == null)
                throw new ArgumentException("Could not find ListFieldIterator control template.");
        }

        private SPControlMode GetControlMode(string fieldName)
        {
            FieldDisplayRuleItem rule = _dynamicRules.FirstOrDefault( 
                e => ((e.ControlModes == null) || (e.ControlModes.Contains(ControlMode))) &&
                ((e.FieldNames == null) || (e.FieldNames.Contains(fieldName))) &&
                (e.Rule == FieldDisplayRule.Display));

            SPControlMode result = (rule == null) ? ControlMode : SPControlMode.Display;

            return result;
        }

        protected BaseFieldControl GetFieldControlByName(String fieldNameToFind)
        {
            foreach (Control control in _formContext.FieldControlCollection)
            {
                if (control is BaseFieldControl)
                {
                    BaseFieldControl baseField = (BaseFieldControl)control;
                    String fieldName = baseField.FieldName;
                    if ((fieldName == fieldNameToFind) &&
                        (GetIteratorByFieldControl(baseField).ClientID == ClientID))
                    {
                        return baseField;
                    }
                }
            }
            return null;
        }

        protected FormField GetFormFieldControlByName(Control root, string fieldNameToFind)
        {
            FormField result = null;

            foreach (Control control in root.Controls)
            {
                if (control is FormField && control.Visible)
                {
                    FormField formField = control as FormField;
                    if (formField.FieldName == fieldNameToFind)
                    {
                        result = formField;
                        break;
                    }
                }
                else
                {
                    result = GetFormFieldControlByName(control, fieldNameToFind);
                    if (result != null) break;
                }
            }

            return result;
        }


        protected ListFieldIterator GetIteratorByFieldControl(BaseFieldControl fieldControl)
        {
            return (ListFieldIterator)fieldControl.Parent.Parent.Parent.Parent.Parent;
        }

        protected void SetValidationError(String fieldName, String errorMessage)
        {
            BaseFieldControl fieldControl = GetFieldControlByName(fieldName);
            fieldControl.ErrorMessage = errorMessage;
            fieldControl.IsValid = false;
        }

    }
}
