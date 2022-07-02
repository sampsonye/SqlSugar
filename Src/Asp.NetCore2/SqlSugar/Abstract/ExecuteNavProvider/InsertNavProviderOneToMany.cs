﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar 
{
    public partial class InsertNavProvider<Root, T> where T : class, new() where Root : class, new()
    {

        private void InsertOneToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            List<TChild> children = new List<TChild>();
            var parentEntity = _ParentEntity;
            var parentList = _ParentList;
            var parentNavigateProperty = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == name);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            var thisPkColumn = GetPkColumnByNav(thisEntity, nav);
            var thisFkColumn= GetFKColumnByNav(thisEntity, nav);
            EntityColumnInfo parentPkColumn = GetParentPkColumn();
            foreach (var item in parentList)
            {
                var parentValue = parentPkColumn.PropertyInfo.GetValue(item);
                var childs = parentNavigateProperty.PropertyInfo.GetValue(item) as List<TChild>;
                if (childs != null)
                {
                    foreach (var child in childs)
                    {
                        thisFkColumn.PropertyInfo.SetValue(child, parentValue, null);
                    }
                    children.AddRange(childs);
                }
            }
            InsertDatas(children, thisPkColumn);
            SetNewParent<TChild>(thisEntity,thisPkColumn);
        }

        private EntityColumnInfo GetParentPkColumn()
        {
            EntityColumnInfo parentPkColumn = _ParentPkColumn;
            if (_ParentPkColumn == null)
            {
                _ParentPkColumn= this._ParentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            }

            return parentPkColumn;
        }

        private void SetNewParent<TChild>(EntityInfo entityInfo,EntityColumnInfo entityColumnInfo) where TChild : class, new()
        {
            this._ParentEntity = entityInfo;
            this._ParentPkColumn = entityColumnInfo;
        }
    }
}