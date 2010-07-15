# About
CrmQuery is a tiny domain-specific language for building queries against Microsoft Dynamics CRM. The structure is inspired by method-chaining libraries like jQuery and the syntax is inspired by plain SQL. CrmQuery wraps the QueryExpression interface and supporting classes, aiming to making queries easier to read and write.

#Synopsis

The following idiomatic example query traverses a parent-child tree relationships among instances of the same entity. In Sql this would be a self-join. In Crm it would be a LinkEntity that references the same entity type on either end. In this query we get all of the child nodes of the entity whose name is "hello":

    QueryBase query = QueryDsl
        .Select()
        .From( "myentity" )
        .Join( "myentity", "parentid", "myentity", "myentityid" )
        .Where( "myentity", "name", ConditionOperator.Equal, new object[] { "hello" } )
    .Query;

The corresponding Sql query would look like this:

    select * from 
    myentity e1
    inner join myentity e2
    on e1.parentid = e2.myentityid
    where e1.name = 'hello'
    
Notice that the projection clause is assumed to be *, or in Crm parlance, AllColumns. Passing an instance of AllColumns to Select() will use the instance as the relational projection list.

#Status
CrmQuery is experimental software and should not be used in any production scenarios. If it proves useful further development work may be done, but the present code should be considered to be simply a proof-of-concept.

#Limitations
CrmQuery only works (and and is tested) on relatively simple queries. Joins can be performed only against the root of the query, not against another join. There is a slight impedance mismatch between a function-chaining interface and a set-based language like Sql. Subsequently, the order in which expressions are added can be important. CrmQuery tries to do the right thing by considering the last-added filter and also by walking up the tree of filters in order to find the correct filter to add a criteria to. This logic is crude and may not allow expression of some complex queries.

#License
CrmQuery is provided under the MIT software license. See the file LICENSE for the full text.