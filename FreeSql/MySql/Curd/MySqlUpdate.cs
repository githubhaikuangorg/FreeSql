﻿using FreeSql.Internal;
using FreeSql.Internal.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSql.MySql.Curd {

	class MySqlUpdate<T1> : Internal.CommonProvider.UpdateProvider<T1> where T1 : class {

		public MySqlUpdate(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere)
			: base(orm, commonUtils, commonExpression, dywhere) {
		}

		public override List<T1> ExecuteUpdated() {
			var sql = this.ToSql();
			if (string.IsNullOrEmpty(sql)) return new List<T1>();

			var sb = new StringBuilder();
			sb.Append(sql).Append(" RETURNING ");

			var colidx = 0;
			foreach (var col in _table.Columns.Values) {
				if (colidx > 0) sb.Append(", ");
				sb.Append(_commonUtils.QuoteReadColumn(col.CsType, _commonUtils.QuoteSqlName(col.Attribute.Name))).Append(" as ").Append(_commonUtils.QuoteSqlName(col.CsName));
				++colidx;
			}
			return _orm.Ado.Query<T1>(_transaction, CommandType.Text, sb.ToString(), _params.Concat(_paramsSource).ToArray());
		}
		async public override Task<List<T1>> ExecuteUpdatedAsync() {
			var sql = this.ToSql();
			if (string.IsNullOrEmpty(sql)) return new List<T1>();

			var sb = new StringBuilder();
			sb.Append(sql).Append(" RETURNING ");

			var colidx = 0;
			foreach (var col in _table.Columns.Values) {
				if (colidx > 0) sb.Append(", ");
				sb.Append(_commonUtils.QuoteReadColumn(col.CsType, _commonUtils.QuoteSqlName(col.Attribute.Name))).Append(" as ").Append(_commonUtils.QuoteSqlName(col.CsName));
				++colidx;
			}
			return await _orm.Ado.QueryAsync<T1>(_transaction, CommandType.Text, sb.ToString(), _params.Concat(_paramsSource).ToArray());
		}

		protected override void ToSqlCase(StringBuilder caseWhen, ColumnInfo[] primarys) {
			if (_table.Primarys.Length == 1) {
				caseWhen.Append(_commonUtils.QuoteReadColumn(_table.Primarys.First().CsType, _commonUtils.QuoteSqlName(_table.Primarys.First().Attribute.Name)));
				return;
			}
			caseWhen.Append("CONCAT(");
			var pkidx = 0;
			foreach (var pk in _table.Primarys) {
				if (pkidx > 0) caseWhen.Append(", ");
				caseWhen.Append(_commonUtils.QuoteReadColumn(pk.CsType, _commonUtils.QuoteSqlName(pk.Attribute.Name)));
				++pkidx;
			}
			caseWhen.Append(")");
		}

		protected override void ToSqlWhen(StringBuilder sb, ColumnInfo[] primarys, object d) {
			if (_table.Primarys.Length == 1) {
				sb.Append(_commonUtils.FormatSql("{0}", _table.Properties.TryGetValue(_table.Primarys.First().CsName, out var tryp2) ? tryp2.GetValue(d) : null));
				return;
			}
			sb.Append("CONCAT(");
			var pkidx = 0;
			foreach (var pk in _table.Primarys) {
				if (pkidx > 0) sb.Append(", ");
				sb.Append(_commonUtils.FormatSql("{0}", _table.Properties.TryGetValue(pk.CsName, out var tryp2) ? tryp2.GetValue(d) : null));
				++pkidx;
			}
			sb.Append(")");
		}
	}
}
