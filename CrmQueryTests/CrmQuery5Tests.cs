using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Djn.Testing;
using Djn.Crm5;

namespace Djn.Crm.Test
{
	class CrmQueryTests
	{
		[FestTest]
		public void TestLinkPosition() {

			QueryExpression query = Djn.Crm5.CrmQuery
				.Select( new ColumnSet( true ) )
				.From( "new_dynamicformfield" )
				.Join( "new_dynamicformfield", "new_formfieldid",
					"new_dynamicform", "new_dynamicformid" )
				.Where( "new_dynamicform", "new_name",
					ConditionOperator.Equal, new string[] { "registry" } ).Query;

			Fest.AssertTrue( query.EntityName == "new_dynamicformfield", "Entity name not set" );
			Fest.AssertTrue( ( ( LinkEntity )query.LinkEntities[ 0 ] ).LinkFromEntityName == query.EntityName, "LinkEntity added in incorrect position" );
		}

		[FestTest]
		public void TestLinkPosition2() {

			// two joins, both linking from root entity to separate child entities.
			QueryExpression query = Djn.Crm5.CrmQuery
				.Select( new ColumnSet( true ) )
				.From( "rootentity" )
				.Join( "rootentity", "id",
					"childentity", "parentid" )
				.Join( "rootentity", "id",
					"childentity2", "parentid" ).Query;

			Fest.AssertTrue( query.EntityName == "rootentity", "Entity name not set" );
			Fest.AssertTrue( ( ( LinkEntity )query.LinkEntities[ 0 ] ).LinkFromEntityName == query.EntityName, "LinkEntity added in incorrect position" );
			Fest.AssertTrue( ( ( LinkEntity )query.LinkEntities[ 1 ] ).LinkFromEntityName == query.EntityName, "LinkEntity added in incorrect position" );
		}

		[FestTest]
		public void TestLinkPosition3() {

			// two joins, one linked from root, second linked from first
			QueryExpression query = Djn.Crm5.CrmQuery
				.Select( new ColumnSet( true ) )
				.From( "rootentity" )
				.Join( "rootentity", "id",
					"childentity", "parentid" )
				.Join( "childentity", "id",
					"childentity2", "parentid" ).Query;

			LinkEntity le1 = ( LinkEntity )query.LinkEntities[ 0 ];
			LinkEntity le2 = ( LinkEntity )le1.LinkEntities[ 0 ];

			Fest.AssertTrue( query.EntityName == "rootentity", "Entity name not set" );
			Fest.AssertTrue( le1.LinkFromEntityName == query.EntityName, "LinkEntity added in incorrect position" );
			Fest.AssertTrue( le2.LinkFromEntityName == le1.LinkToEntityName, "LinkEntity added in incorrect position" );
		}

		[FestTest]
		public void TestConditionPosition() {

			// two joins, one linked from root, second linked from first
			// putting a where clause in between.
			QueryExpression query = Djn.Crm5.CrmQuery
				.Select( new ColumnSet( true ) )
				.From( "rootentity" )
				.Join( "rootentity", "id",
					"childentity", "parentid" )
				.Where( "childentity", "myproperty", ConditionOperator.Equal, new object[] { "val1" } )
				.Join( "childentity", "id",
					"childentity2", "parentid" ).Query;

			LinkEntity le1 = ( LinkEntity )query.LinkEntities[ 0 ];
			LinkEntity le2 = ( LinkEntity )le1.LinkEntities[ 0 ];
			// TODO: this brings up an interesting issue - where do we want to put criteria when adding via 'Where()'. 
			// Currently they end up getting a new Filter under LinkCriteria rather than added to the existing FilterExpression.
			ConditionExpression ce = ( ConditionExpression )( ( FilterExpression )le1.LinkCriteria.Filters[ 0 ] ).Conditions[ 0 ];

			Fest.AssertTrue( query.EntityName == "rootentity", "Entity name not set" );
			Fest.AssertTrue( le1.LinkFromEntityName == query.EntityName, "LinkEntity added in incorrect position" );
			Fest.AssertTrue( le2.LinkFromEntityName == le1.LinkToEntityName, "LinkEntity added in incorrect position" );
			Fest.AssertTrue( ce.AttributeName == "myproperty", "ConditionExpression added in incorrect position" );
		}

		[FestTest]
		public void TestOrOperator() {

			QueryExpression query = Djn.Crm5.CrmQuery
				.Select( new ColumnSet( true ) )
				.From( "altai_dynamicformfield" )
				.Where( "altai_dynamicformfield", "altai_name",
					ConditionOperator.Equal, new string[] { "address1" } )
				.Or( "altai_name",
					ConditionOperator.Equal, new string[] { "address2" } ).Query;

			Fest.AssertTrue( query.EntityName == "altai_dynamicformfield", "Entity name not set" );
			// 'live' CRM test removed
			// IOrganizationService service = Altai.MSCrm.CRM5ServiceUtil.GetCrmService();
			// EntityCollection ent = Altai.MSCrm.DynamicEntityHelper5.GetDynamicEntityCollection( service, query );
			// Fest.AssertTrue( ent.Entities.Count == 2, "expected 2 entities" );
		}


		[FestTest]
		/**
		 * Test new constructor that takes variable number of string arguments
		 * and creates a ColumnSet behind the scenes.
		 */
		public void TestStringColumns() {

			QueryExpression query = Djn.Crm5.CrmQuery
				.Select( "altai_dynamicformfieldid", "altai_name" )
				.From( "altai_dynamicformfield" )
				.Where( "altai_dynamicformfield", "altai_name",
					ConditionOperator.Equal, new string[] { "address1" } ).Query;

			Fest.AssertTrue( query.ColumnSet.Columns.Contains( "altai_name" ), "ColumnSet does not contain altai_name" );
			Fest.AssertTrue( query.ColumnSet.Columns.Contains( "altai_dynamicformfieldid" ), "ColumnSet does not contain altai_dynamicformfieldid" );
		}
	} // class
} // namespace
