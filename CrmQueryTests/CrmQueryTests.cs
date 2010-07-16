using System;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Djn.Testing;
using Djn.Crm;

namespace Djn.Crm.Test
{
	class CrmQueryTests
	{
		[FestTest]
		public void TestLinkPosition() {

			QueryExpression query = CrmQuery
				.Select( new AllColumns() )
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
			QueryExpression query = CrmQuery
				.Select( new AllColumns() )
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
			QueryExpression query = CrmQuery
				.Select( new AllColumns() )
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
			QueryExpression query = CrmQuery
				.Select( new AllColumns() )
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
	}
}
