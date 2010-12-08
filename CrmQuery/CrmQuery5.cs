using System;
using System.Collections;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Djn.Crm5
{
	/**
	* CrmQuery is an experimental domain-specific language for building
	*	Microsoft CRM QueryExpressions.
	* 
	* this software is provided under the MIT license. See file LICENSE
	*	for details.
	*	
	* This version of CrmQuery targets the CRM 2011/5.0 product.
	*/
	public class CrmQuery
	{
		// We use Select() function instead of constructor
		private CrmQuery() { }

		/**
		 * CrmQuery wraps a CRM QueryExpression. Idomatic usage chains calls 
		 * together, only accessing the Query as the last call in the chain.
		 */
		public QueryExpression Query {
			get { return m_query; }
		}
		private QueryExpression m_query;

		/**
		 * lastAddedLink is a way to let us add filters to a specific
		 * LinkEntity by adding things in order. Allows self-joins.
		 */
		private LinkEntity m_lastAddedLink;

		/**
		 * Select serves as the constructor and the start of the 
		 * chain. By Sql convention, accepts the projection list
		 */
		public static CrmQuery Select( ColumnSet in_columns ) {
			QueryExpression query = new QueryExpression();
			query.ColumnSet = in_columns;
			CrmQuery dsl = new CrmQuery();
			dsl.m_query = query;
			return dsl;
		}
		public static CrmQuery Select() {
			return Select( new ColumnSet(true ) );
		}

		/**
		 * From sets the entity type that the query will return
		 */
		public CrmQuery From( string in_entityName ) {
			m_query.EntityName = in_entityName;
			return this;
		}

		/**
		 * Join uses LinkEntity to establish a relation between two entities.
		 * Use Where to add criteria using the 'to' entity.
		 */
		public static LinkEntity JoinExpression( string in_fromEntity, string in_fromField, string in_toEntity, string in_toField ) {
			LinkEntity linkEntity = new LinkEntity();
			linkEntity.LinkFromEntityName = in_fromEntity;
			linkEntity.LinkFromAttributeName = in_fromField;
			linkEntity.LinkToEntityName = in_toEntity;
			linkEntity.LinkToAttributeName = in_toField;
			linkEntity.JoinOperator = JoinOperator.Inner;
			return linkEntity;
		}
		public CrmQuery Join( string in_fromEntity, string in_fromField, string in_toEntity, string in_toField ) {
			LinkEntity linkEntity = JoinExpression( in_fromEntity, in_fromField, in_toEntity, in_toField ) ;
			return Join( in_fromEntity, linkEntity );
		}
		public CrmQuery Join( string in_fromEntity, LinkEntity in_linkEntity ) {
			// TODO: We look at the root query first. Double-think is this the right thing?
			if( m_query.EntityName == in_fromEntity ) {
				m_query.LinkEntities.Add( in_linkEntity );
			}
			else {
				LinkEntity link = FindEntityLink( m_query.LinkEntities, in_fromEntity );
				if( link != null ) {
					link.LinkEntities.Add( in_linkEntity );
				}
			}
			m_lastAddedLink = in_linkEntity;
			return this;
		}

		public static FilterExpression WhereExpression( string in_field, ConditionOperator in_operator, object[] in_values ) {
			FilterExpression filterExpression = new FilterExpression();
			filterExpression.FilterOperator = LogicalOperator.And;

			ConditionExpression ce = new ConditionExpression();
			ce.AttributeName = in_field;
			ce.Operator = in_operator;
            foreach (object item in in_values) {
                ce.Values.Add(item);
            }

			filterExpression.Conditions.Add( ce );
			return filterExpression;
		}
		public CrmQuery Where( string in_entity, string in_field, ConditionOperator in_operator, object[] in_values ) {
			FilterExpression filterExpression = CrmQuery.WhereExpression( in_field, in_operator, in_values );
			return Where( in_entity, filterExpression );
		}
		public CrmQuery Where( string in_entity, FilterExpression in_filterExpression ) {
			// TODO: this logic is repeated in Join()
			if( m_lastAddedLink != null ) {
				m_lastAddedLink.LinkCriteria.AddFilter( in_filterExpression );
			}
			else if( m_query.EntityName == in_entity ) {
				m_query.Criteria.AddFilter( in_filterExpression );
			}
			else {
				LinkEntity link = FindEntityLink( m_query.LinkEntities, in_entity );
				if( link != null ) {
					link.LinkCriteria.AddFilter( in_filterExpression );
				}
			}
			return this;
		}

		/**
		 * Used by Where to figure out which LinkEntity corresponds to the desired
		 * entity we wish to attach the criteria to
		 */
		private LinkEntity FindEntityLink( DataCollection<LinkEntity> in_linkEntities, string in_entityName ) {
			foreach( LinkEntity link in in_linkEntities ) {
				FindEntityLink( link.LinkEntities, in_entityName );
				if( link.LinkToEntityName == in_entityName ) {
					return link;
				}
			}
			return null;
		}

		/**
		 * OrderBy adds ordering fields to the query at the toplevel.
		 * 
		 * TODO: for full sql compliance we need to pass array of booleans
		 * since we can specify ascending/descending for each field
		 */
		public CrmQuery OrderBy( string[] in_orderfields, OrderType in_ordertype ) {
			foreach( String orderfield in in_orderfields ) {
				if( ( orderfield != null ) && ( orderfield != "" ) ) {
					m_query.AddOrder( orderfield, in_ordertype );
				}
			}
			return this;
		}

	} // class 
} // namespace