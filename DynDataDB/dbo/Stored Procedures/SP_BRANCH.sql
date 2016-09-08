CREATE PROCEDURE SP_BRANCH
as

MERGE INTO branch AS target
USING branch_stg AS source
    ON target.branchid = source.branchid
WHEN NOT MATCHED BY TARGET THEN
    INSERT (branchid)
    VALUES (source.branchid);
    
truncate table branch_stg