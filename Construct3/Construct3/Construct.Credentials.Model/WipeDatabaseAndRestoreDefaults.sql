/****** Object:  StoredProcedure [dbo].[WipeDatabaseAndResetToDefaults]    Script Date: 07/18/2014 10:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[WipeDatabaseAndResetToDefaults]
AS 
    BEGIN
-----------------------------================= DataTypeSources
-- disable all constraints
        EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"

-- delete data in all tables
        EXEC sp_MSForEachTable "DELETE FROM ?"

-- enable all constraints
        EXEC sp_msforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"

        DECLARE @cmd VARCHAR(4000)
        DECLARE cmds CURSOR
        FOR
            SELECT  'drop table [' + Table_Name + ']'
            FROM    INFORMATION_SCHEMA.TABLES
            WHERE   Table_Name LIKE 'z_%'
            UNION
            SELECT  'drop view [' + Table_Name + ']'
            FROM    INFORMATION_SCHEMA.VIEWS
            WHERE   Table_Name LIKE 'z_%'
    
        OPEN cmds
        WHILE 1 = 1 
            BEGIN
                FETCH cmds INTO @cmd
                IF @@fetch_status != 0 
                    BREAK
                EXEC(@cmd)
            END
    

        DEALLOCATE cmds

        DECLARE @procName VARCHAR(500)
 
        DECLARE cur CURSOR
        FOR
            SELECT  [name]
            FROM    sys.objects
            WHERE   type = 'p' AND name LIKE 'z_%'
 
        OPEN cur

        FETCH NEXT FROM cur INTO @procName
 
        WHILE @@fetch_status = 0 
            BEGIN
 
                EXEC('drop procedure ' + @procName)
 
                FETCH NEXT FROM cur INTO @procName
 
            END
 
        CLOSE cur
 
        DEALLOCATE cur

    
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'FF892A13-8363-472C-B39A-9258081EA963' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'FF892A13-8363-472C-B39A-9258081EA963' ,
                          NULL ,
                          N'External' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                  
                        )
                VALUES  ( N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          NULL ,
                          N'Internal' ,
                          1 ,
                          1
                        )
            END
		
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'C33831DF-A092-4898-81E8-03276C4F3B30' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'C33831DF-A092-4898-81E8-03276C4F3B30' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'Query' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'5C11FBBD-9E36-4BEA-A8BE-06E225250EF8' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'5C11FBBD-9E36-4BEA-A8BE-06E225250EF8' ,
                          N'FF892A13-8363-472C-B39A-9258081EA963' ,
                          N'Sensor' ,
                          1 ,
                          1
                        )
            END	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'3BF44DE6-E03A-4E73-B5A0-277EF3F724B8' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'3BF44DE6-E03A-4E73-B5A0-277EF3F724B8' ,
                          N'FF892A13-8363-472C-B39A-9258081EA963' ,
                          N'Human' ,
                          1 ,
                          1
                        )
            END	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'DD8904F7-6508-4537-80B4-9C330DEF6083' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'DD8904F7-6508-4537-80B4-9C330DEF6083' ,
                          N'C33831DF-A092-4898-81E8-03276C4F3B30' ,
                          N'User-Defined Query' ,
                          1 ,
                          1
                        )
            END	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'5C8B0DC2-EF4F-4C71-80FD-CB518606E1DE' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'5C8B0DC2-EF4F-4C71-80FD-CB518606E1DE' ,
                          N'C33831DF-A092-4898-81E8-03276C4F3B30' ,
                          N'Auto-Generated Query' ,
                          1 ,
                          1
                        )
            END	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'E77FC56E-4DF0-42A3-9901-F37238BB548D' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'E77FC56E-4DF0-42A3-9901-F37238BB548D' ,
                          N'FF892A13-8363-472C-B39A-9258081EA963' ,
                          N'Media Stream' ,
                          1 ,
                          1
                        )
            END	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'204C56E2-BAF1-424C-BCC8-8BB736D3EAF6' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'204C56E2-BAF1-424C-BCC8-8BB736D3EAF6' ,
                          N'3BF44DE6-E03A-4E73-B5A0-277EF3F724B8' ,
                          N'Constant' ,
                          1 ,
                          1
                        )
            END
		
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'442D54C5-3D51-4169-A5B7-D382082037CD' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'442D54C5-3D51-4169-A5B7-D382082037CD' ,
                          N'204C56E2-BAF1-424C-BCC8-8BB736D3EAF6' ,
                          N'Constant for Analytics' ,
                          0 ,
                          1
                        )
            END	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'260E63F9-2140-434A-BAF6-C4632428A4EE' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'260E63F9-2140-434A-BAF6-C4632428A4EE' ,
                          N'204C56E2-BAF1-424C-BCC8-8BB736D3EAF6' ,
                          N'Constant for SessionContexts' ,
                          0 ,
                          1
                        )
            END	
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'dc74447f-44bc-4761-bb72-1828d093c986' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'dc74447f-44bc-4761-bb72-1828d093c986' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'Question' ,
                          1 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'9350cc80-963d-413f-8753-adbc791f6dc2' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'9350cc80-963d-413f-8753-adbc791f6dc2' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'Aggregator' ,
                          1 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'434062d7-c37b-4bfb-8a9d-7ff68294ae93' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'434062d7-c37b-4bfb-8a9d-7ff68294ae93' ,
                          N'9350cc80-963d-413f-8753-adbc791f6dc2' ,
                          N'Numeric_Aggregator' ,
                          1 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'06eea444-f884-435c-850f-31688097f13d' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'06eea444-f884-435c-850f-31688097f13d' ,
                          N'434062d7-c37b-4bfb-8a9d-7ff68294ae93' ,
                          N'Numeric_Aggregator_Average' ,
                          0 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'2e1b1216-5ef2-4aec-9ca4-28b986452b3f' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'2e1b1216-5ef2-4aec-9ca4-28b986452b3f' ,
                          N'434062d7-c37b-4bfb-8a9d-7ff68294ae93' ,
                          N'Numeric_Aggregator_Median' ,
                          0 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'70561e1f-8088-43f4-a07e-ad15d9484530' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'70561e1f-8088-43f4-a07e-ad15d9484530' ,
                          N'434062d7-c37b-4bfb-8a9d-7ff68294ae93' ,
                          N'Numeric_Aggregator_Min' ,
                          0 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'ce60c81c-b59c-45f2-8b5c-37029ed37a16' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'ce60c81c-b59c-45f2-8b5c-37029ed37a16' ,
                          N'434062d7-c37b-4bfb-8a9d-7ff68294ae93' ,
                          N'Numeric_Aggregator_Max' ,
                          0 ,
                          1
                        )
            END

		IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_DataTypeSource]
                        WHERE   [ID] = N'064A944C-9347-4E0A-9642-744F80D7BD8F' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_DataTypeSource]
                        ( [ID] ,
                          [ParentID] ,
                          [Name] ,
                          [IsCategory] ,
                          [IsReadOnly]
                        )
                VALUES  ( N'064A944C-9347-4E0A-9642-744F80D7BD8F' ,
                          N'3BF44DE6-E03A-4E73-B5A0-277EF3F724B8' ,
                          N'Supervised_Machine_Learning_Labeler' ,
                          0,
                          1
                        )
            END	
	

-----------------------------================= SENSORHOSTTYPES



-- Add 7 rows to [dbo].[Sources_SensorHostType]
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'2f023f02-621c-464a-aa21-a921eb4fc627' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'2f023f02-621c-464a-aa21-a921eb4fc627' ,
                          N'Workstation OS' ,
                          NULL ,
                          1
                        )
            END
	 
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'96427bb1-798b-427c-8999-bdabfc3bcaf8' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'96427bb1-798b-427c-8999-bdabfc3bcaf8' ,
                          N'Mobile OS' ,
                          NULL ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'895ec14f-9450-4102-b406-4562ad980c0a' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'895ec14f-9450-4102-b406-4562ad980c0a' ,
                          N'Embedded' ,
                          '96427bb1-798b-427c-8999-bdabfc3bcaf8' ,
                          1
                        )
            END

        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'aa333392-e904-436f-ab7d-0a4289ceca46' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'aa333392-e904-436f-ab7d-0a4289ceca46' ,
                          N'iPhone' ,
                          '96427bb1-798b-427c-8999-bdabfc3bcaf8' ,
                          0
                        )
            END


   
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'6171d2fe-991d-4903-a272-4b0b50b97c31' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'6171d2fe-991d-4903-a272-4b0b50b97c31' ,
                          N'Windows Phone' ,
                          '96427bb1-798b-427c-8999-bdabfc3bcaf8' ,
                          0
                        )
            END
	
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'c0023c17-5853-4e49-82bc-e1de4eb502f2' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'c0023c17-5853-4e49-82bc-e1de4eb502f2' ,
                          N'Android' ,
                          NULL ,
                          0
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Sources_SensorHostType]
                        WHERE   [ID] = N'eda0ff3e-108b-45d5-bf58-f362fabf2efe' ) 
            BEGIN
                INSERT  INTO [dbo].[Sources_SensorHostType]
                        ( [ID] ,
                          [SensorHostTypeName] ,
                          [ParentID] ,
                          [IsCategory]
                        )
                VALUES  ( N'eda0ff3e-108b-45d5-bf58-f362fabf2efe' ,
                          N'Windows' ,
                          '2f023f02-621c-464a-aa21-a921eb4fc627' ,
                          0
                        )
            END
	
	
	------------------------------------DATATYPES
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'B688A8BB-5E2E-4440-A468-B0F23BDF1955' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'B688A8BB-5E2E-4440-A468-B0F23BDF1955' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'Object' ,
                          N'System.Object' ,
                          1 ,
                          1 
                        )
            END	
    
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'2b0becbf-d814-4bb2-8fab-56c0723b4e94' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'2b0becbf-d814-4bb2-8fab-56c0723b4e94' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'bool' ,
                          N'System.Boolean' ,
                          1 ,
                          1 
                        )
            END
		
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'64b84e4a-6545-405f-b760-5ca96d15ec3e' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'64b84e4a-6545-405f-b760-5ca96d15ec3e' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'Guid' ,
                          N'System.Guid' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'a0e1d769-5742-41a2-b34a-adf2cc88031e' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'a0e1d769-5742-41a2-b34a-adf2cc88031e' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'float' ,
                          N'System.Single' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'a6ffa473-3483-43b5-a2a8-b606c565c883' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'a6ffa473-3483-43b5-a2a8-b606c565c883' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'int' ,
                          N'System.Int32' ,
                          1 ,
                          1
                        )
            END
	
	    IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'565df1e1-b9f3-4b3e-9228-7625bd8817d4' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'565df1e1-b9f3-4b3e-9228-7625bd8817d4' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'long' ,
                          N'System.Int64' ,
                          1 ,
                          1
                        )
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'c8d37d33-1f38-4ec5-9d49-c28df3466b00' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'c8d37d33-1f38-4ec5-9d49-c28df3466b00' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'DateTime' ,
                          N'System.DateTime' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'2f08c723-3eea-4838-9a2a-587e3835ff5e' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'2f08c723-3eea-4838-9a2a-587e3835ff5e' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'IEnumerable' ,
                          N'System.Collections.Generic.IEnumerable' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'C0FF00A6-7675-4737-94A0-593A79A05305' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'C0FF00A6-7675-4737-94A0-593A79A05305' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'string' ,
                          N'System.String' ,
                          1 ,
                          1
                        )
            END
	
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'FB826919-232D-47B1-A77B-794E0615C92E' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'FB826919-232D-47B1-A77B-794E0615C92E' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'double' ,
                          N'System.Double' ,
                          1 ,
                          1
                        )
            END

        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Types_DataType]
                        WHERE   [ID] = N'41C2B03D-149D-46FD-B9F2-8CB87E8ABDAA' ) 
            BEGIN
                INSERT  INTO [dbo].[Types_DataType]
                        ( [ID] ,
                          [DataTypeSourceID] ,
                          [Name] ,
                          [FullName] ,
                          IsReadOnly ,
                          IsCoreType
                        )
                VALUES  ( N'41C2B03D-149D-46FD-B9F2-8CB87E8ABDAA' ,
                          N'5520348D-BF4E-46FA-B603-F26A46AE2C73' ,
                          N'byte[]' ,
                          N'System.Byte[]' ,
                          1 ,
                          1
                        )
                
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Visualizations_Visualizer]
                        WHERE   [ID] = N'937EC2ED-DD17-492C-87D3-650B01F14519' ) 
            BEGIN
                INSERT  INTO [dbo].[Visualizations_Visualizer]
                        ( [ID] ,
                          [PrimitiveDataTypeID] ,
                          [Name] ,
                          [Description]
                        )
                VALUES  ( N'937EC2ED-DD17-492C-87D3-650B01F14519' ,
                          N'A6FFA473-3483-43B5-A2A8-B606C565C883' ,
                          N'IntVisualizer' ,
                          N'Int Visualizer'
                        )
                
            END
            
        IF NOT EXISTS ( SELECT  *
                        FROM    [dbo].[Visualizations_Visualizer]
                        WHERE   [ID] = N'7FBD410A-CC7A-47B2-895D-48520909548A' ) 
            BEGIN
                INSERT  INTO [dbo].[Visualizations_Visualizer]
                        ( [ID] ,
                          [PrimitiveDataTypeID] ,
                          [Name] ,
                          [Description]
                        )
                VALUES  ( N'7FBD410A-CC7A-47B2-895D-48520909548A' ,
                          N'2B0BECBF-D814-4BB2-8FAB-56C0723B4E94' ,
                          N'BooleanVisualizer' ,
                          N'Bool Visualizer'
                        )
                
            END
            
    END