using PX.Data;
using System;
using System.Collections.Generic;
using PX.Common;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.GL;


namespace MainProject.Attributes
{
    [PXDBInt]
    [PXInt]
    [PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, FieldClass = "SUBACCOUNT")]
    [PXRestrictor(typeof(Where<Sub.active, Equal<True>>), "Subaccount {0} is inactive.", new Type[] { typeof(Sub.subCD) })]
    public class AllSubAccountSelectorAttribute : PXEntityAttribute, IPXRowInsertingSubscriber
    {
        public class dimensionName : BqlType<IBqlString, string>.Constant<dimensionName>
        {
            public dimensionName()
                : base("SUBACCOUNT")
            {
            }
        }

        protected class Definition : IPrefetchable, IPXCompanyDependent
        {
            private int? _DefaultSubID;

            public int? DefaultSubID => _DefaultSubID;

            public void Prefetch()
            {
                _DefaultSubID = null;
                using (PXDataRecord pXDataRecord = PXDatabase.SelectSingle<Sub>(new PXDataField[2]
                {
                new PXDataField<Sub.subID>(),
                new PXDataFieldOrder<Sub.subID>()
                }))
                {
                    if (pXDataRecord != null)
                    {
                        _DefaultSubID = pXDataRecord.GetInt32(0);
                    }
                };
                
            }
        }

        public const string DimensionName = "SUBACCOUNT";

        private readonly Type _branchID;

        private readonly Type _accounType;

        protected static Definition Definitions
        {
            get
            {
                Definition definition = PXContext.GetSlot<Definition>();
                if (definition == null)
                {
                    definition = PXContext.SetSlot(PXDatabase.GetSlot<Definition>(typeof(Definition).FullName, new Type[1] { typeof(Sub) }));
                }

                return definition;
            }
        }

        public AllSubAccountSelectorAttribute()
            : this(null)
        {
        }

        public AllSubAccountSelectorAttribute(Type AccountType)
            : this(AccountType, null)
        {
        }

        public AllSubAccountSelectorAttribute(Type AccountType, Type BranchType, bool AccountAndBranchRequired = false)
            : this(typeof(Search<Sub.subID>), AccountType, BranchType, AccountAndBranchRequired)
        {
        }

        public AllSubAccountSelectorAttribute(Type SearchType, Type AccountType, Type BranchType, bool AccountAndBranchRequired = false)
        {
            if (SearchType == null)
            {
                throw new PXArgumentException("SearchType", "The argument cannot be null.");
            }

            List<Type> list = new List<Type> { SearchType.GetGenericTypeDefinition() };
            list.AddRange(SearchType.GetGenericArguments());
            for (int i = 0; i < list.Count; i++)
            {
                if (typeof(IBqlWhere).IsAssignableFrom(list[i]) && AccountType != null)
                {
                    _accounType = AccountType;
                    list[i] = BqlCommand.Compose(typeof(Where2<,>), GetIsNullAndMatchWhere(AccountType, AccountAndBranchRequired), typeof(And<>), list[i]);
                    if (BranchType != null)
                    {
                        _branchID = BranchType;
                        list[i] = BqlCommand.Compose(typeof(Where2<,>), GetIsNullAndMatchWhere(BranchType, AccountAndBranchRequired), typeof(And<>), list[i]);
                    }

                    SearchType = BqlCommand.Compose(list.ToArray());
                }
            }

            PXDimensionSelectorAttribute item = new PXDimensionSelectorAttribute("SUBACCOUNT", SearchType, typeof(Sub.subCD))
            {
                CacheGlobal = true,
                DescriptionField = typeof(Sub.description)
            };
            _Attributes.Add(item);
            _SelAttrIndex = _Attributes.Count - 1;
            Filterable = true;
        }

        private static Type GetIsNullAndMatchWhere(Type entityType, bool IsRequired)
        {
            Type type = BqlCommand.Compose(typeof(Where<>), typeof(Match<>), typeof(Optional<>), entityType);
            if (!IsRequired)
            {
                type = BqlCommand.Compose(typeof(Where<,,>), typeof(Optional<>), entityType, typeof(IsNull), typeof(Or<>), type);
            }

            return type;
        }

        public override void CacheAttached(PXCache sender)
        {
            if (!PXAccess.FeatureInstalled<FeaturesSet.subAccount>())
            {
                ((PXDimensionSelectorAttribute)_Attributes[_Attributes.Count - 1]).ValidComboRequired = false;
                sender.Graph.FieldDefaulting.AddHandler(sender.GetItemType(), _FieldName, FieldDefaulting);
            }

            base.CacheAttached(sender);
            if (_branchID != null)
            {
                sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_branchID), _branchID.Name, RelatedFieldUpdated);
            }

            if (_accounType != null)
            {
                sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_accounType), _accounType.Name, RelatedFieldUpdated);
            }

            sender.Graph.RowPersisting.AddHandler(sender.GetItemType(), RowPersisting);
        }

        public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            int? num = (int?)sender.GetValue(e.Row, _FieldOrdinal);
            if (((e.Operation & PXDBOperation.Delete) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Delete) == PXDBOperation.Update) && !num.HasValue && !PXAccess.FeatureInstalled<FeaturesSet.subAccount>())
            {
                num = GetDefaultSubID(sender, e.Row);
                sender.SetValue(e.Row, _FieldName, num);
                PXUIFieldAttribute.SetError(sender, e.Row, _FieldName, null);
            }

            if (((e.Operation & PXDBOperation.Delete) != PXDBOperation.Insert && (e.Operation & PXDBOperation.Delete) != PXDBOperation.Update) || !(num < 0))
            {
                return;
            }

            PXCache pXCache = sender.Graph.Caches[typeof(Sub)];
            PXSelectBase<Sub> pXSelectBase = new PXSelectReadonly<Sub, Where<Sub.subCD, Equal<Current<Sub.subCD>>>>(sender.Graph);
            Sub sub = null;
            foreach (Sub item in pXCache.Inserted)
            {
                if (object.Equals(item.SubID, num) && (sub = (Sub)pXSelectBase.View.SelectSingleBound(new object[1] { item })) != null)
                {
                    pXCache.RaiseRowPersisting(item, PXDBOperation.Insert);
                    pXCache.RaiseRowPersisted(sub, PXDBOperation.Insert, PXTranStatus.Open, null);
                    sub = item;
                    break;
                }
            }

            if (sub != null)
            {
                pXCache.SetStatus(sub, PXEntryStatus.Notchanged);
                pXCache.Remove(sub);
            }
        }

        protected virtual void RelatedFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SetSubAccount(sender, e.Row);
        }

        public virtual void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            SetSubAccount(sender, e.Row);
        }

        public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (!e.Cancel)
            {
                e.NewValue = GetDefaultSubID(sender, e.Row);
                e.Cancel = true;
            }
        }

        private void SetSubAccount(PXCache sender, object row)
        {
            PXFieldState pXFieldState = (PXFieldState)sender.GetValueExt(row, _FieldName);
            if (pXFieldState == null || pXFieldState.Value == null)
            {
                return;
            }

            sender.SetValue(row, _FieldName, null);
            sender.SetValueExt(row, _FieldName, pXFieldState.Value);
            if (FieldErrorScope.NeedsReset(sender.GetBqlField(_FieldName).FullName))
            {
                pXFieldState = (PXFieldState)sender.GetValueExt(row, _FieldName);
                if (sender.GetValue(row, _FieldName) == null && pXFieldState != null && pXFieldState.ErrorLevel >= PXErrorLevel.Error)
                {
                    sender.RaiseExceptionHandling(_FieldName, row, null, null);
                }
            }
        }

        private int? GetDefaultSubID(PXCache sender, object row)
        {
            if (!Definitions.DefaultSubID.HasValue)
            {
                object newValue = "0";
                sender.RaiseFieldUpdating(_FieldName, row, ref newValue);
                return (int?)newValue;
            }

            return Definitions.DefaultSubID;
        }

        public static Sub GetSubaccount(PXGraph graph, int? subID)
        {
            if (!subID.HasValue)
            {
                throw new ArgumentNullException("subID");
            }

            Sub sub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(graph, subID);
            if (sub == null)
            {
                throw new PXException("{0} with ID '{1}' does not exists", EntityHelper.GetFriendlyEntityName(typeof(Sub)), subID);
            }

            return sub;
        }

        //
        // Summary:
        //     Returns deafult subID if default subaccount exists, else returns null.
        public static int? TryGetDefaultSubID()
        {
            return Definitions.DefaultSubID;
        }
    }
}
