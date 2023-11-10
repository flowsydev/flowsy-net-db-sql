namespace Flowsy.Db.Sql;

public static class DbStateCodes
{
    public static class SuccessfulCompletion
    {
        public const string Category = "S";
        public const string Class = "00";
        public const string NoSubclass = $"{Class}000";
    }

    public static class Warning
    {
        public const string Category = "W";
        public const string Class = "01";
        public const string NoSubclass = $"{Class}000";

        public const string CursorOperationConflict = $"{Class}001";
        public const string DisconnectError = $"{Class}002";
        public const string NullValueEliminatedInSetFunction = $"{Class}003";
        public const string StringDataRightTruncation = $"{Class}004";
        public const string InsufficientItemDescriptorAreas = $"{Class}005";
        public const string PrivilegeNotRevoked = $"{Class}006";
        public const string PrivilegeNotGranted = $"{Class}007";
        public const string SearchConditionTooLongForInformationSchema = $"{Class}009";
        public const string QueryExpressionTooLongForInformationSchema = $"{Class}00A";
        public const string DefaultValueTooLongForInformationSchema = $"{Class}00B";
        public const string ResultSetsReturned = $"{Class}00C";
        public const string AdditionalResultSetsReturned = $"{Class}00D";
        public const string AttemptToReturnTooManyResultSets = $"{Class}00E";
        public const string StatementTooLongForInformationSchema = $"{Class}00F";
        public const string ColumnCannotBeMapped = $"{Class}010";
        public const string SqlJavaPathTooLongForInformationSchema = $"{Class}011";
        public const string InvalidNumberOfConditions = $"{Class}012";
        public const string ArrayDataRightTruncation = $"{Class}02F";
    }

    public static class NoData
    {
        public const string Category = "N";
        public const string Class = "02";
        public const string NoSubclass = $"{Class}000";
        public const string NoAdditionalResultSetsReturned = $"{Class}001";
    }

    public static class DynamicSqlError
    {
        public const string Category = "X";
        public const string Class = "07";
        public const string NoSubclass = $"{Class}000";
        public const string UsingClauseDoesNotMatchDynamicParameterSpecifications = $"{Class}001";
        public const string UsingClauseDoesNotMatchTargetSpecifications = $"{Class}002";
        public const string CursorSpecificationCannotBeExecuted = $"{Class}003";
        public const string UsingClauseRequiredForDynamicParameters = $"{Class}004";
        public const string PreparedStatementNotACursorSpecification = $"{Class}005";
        public const string RestrictedDataTypeAttributeViolation = $"{Class}006";
        public const string UsingClauseRequiredForResultFields = $"{Class}007";
        public const string InvalidDescriptorCount = $"{Class}008";
        public const string InvalidDescriptorIndex = $"{Class}009";
        public const string DataTypeTransformFunctionViolation = $"{Class}00B";
        public const string UndefinedDataValue = $"{Class}00C";
        public const string InvalidDataTarget = $"{Class}00D";
        public const string InvalidLevelValue = $"{Class}00E";
        public const string InvalidDatetimeIntervalCode = $"{Class}00F";
    }

    public static class ConnectionException
    {
        public const string Category = "X";
        public const string Class = "08";
        public const string NoSubclass = $"{Class}000";
        public const string SqlClientUnableToEstablishSqlConnection = $"{Class}001";
        public const string ConnectionNameInUse = $"{Class}002";
        public const string ConnectionDoesNotExist = $"{Class}003";
        public const string SqlServerRejectedEstablishmentOfSqlConnection = $"{Class}004";
        public const string ConnectionFailure = $"{Class}006";
        public const string TransactionResolutionUnknown = $"{Class}007";
    }

    public static class TriggeredActionException
    {
        public const string Category = "X";
        public const string Class = "09";
        public const string NoSubclass = $"{Class}000";   
    }

    public static class FeatureNotSupported
    {
        public const string Category = "X";
        public const string Class = "0A";
        public const string NoSubclass = $"{Class}000";
        public const string MultipleServerTransactions = $"{Class}001";
    }

    public static class InvalidTargetTypeSpecification
    {
        public const string Category = "X";
        public const string Class = "0D";
        public const string NoSubclass = $"{Class}000";
    }

    public static class InvalidSchemaNameListSpecification
    {
        public const string Category = "X";
        public const string Class = "0E";
        public const string NoSubclass = $"{Class}000";
    }

    public static class LocatorException
    {
        public const string Category = "X";
        public const string Class = "0F";
        public const string NoSubclass = $"{Class}000";
        public const string InvalidSpecification = $"{Class}001";
    }

    public static class ResignalWhenHandlerNotActive
    {
        public const string Category = "X";
        public const string Class = "0K";
        public const string NoSubclass = $"{Class}000";   
    }
    
    public static class InvalidGrantor
    {
        public const string Category = "X";
        public const string Class = "0L";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidSqlInvokedProcedureReference
    {
        public const string Category = "X";
        public const string Class = "0M";
        public const string NoSubclass = $"{Class}000";
    }

    public static class SqlXmlMappingError
    {
        public const string Category = "X";
        public const string Class = "0N";
        public const string NoSubclass = $"{Class}000";
        public const string UnmappableXmlName = $"{Class}000";
        public const string InvalidXmlCharacter = $"{Class}000";
    }
    
    public static class InvalidRoleSpecification
    {
        public const string Category = "X";
        public const string Class = "0P";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidTransformGroupNameSpecification
    {
        public const string Category = "X";
        public const string Class = "0S";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class TargetTableDisagreesWithCursorSpecification
    {
        public const string Category = "X";
        public const string Class = "0T";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class AttemptToAssignToNonUpdatableColumn
    {
        public const string Category = "X";
        public const string Class = "0U";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class AttemptToAssignToOrderingColumn
    {
        public const string Category = "X";
        public const string Class = "0V";
        public const string NoSubclass = $"{Class}000";
    }

    public static class ProhibitedStatementEncounteredDuringTriggerExecution
    {
        public const string Category = "X";
        public const string Class = "0W";
        public const string NoSubclass = $"{Class}000";
        public const string ModifyTableModifiedByDataChangeDeltaTable = $"{Class}001";
    }
    
    public static class InvalidForeignServerSpecification
    {
        public const string Category = "X";
        public const string Class = "0X";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class PassThroughSpecificCondition
    {
        public const string Category = "X";
        public const string Class = "0Y";
        public const string NoSubclass = $"{Class}000";
        public const string InvalidCursorOption  = $"{Class}001";
        public const string InvalidCursorAllocation = $"{Class}002";
    }

    public static class DiagnosticsException
    {
        public const string Category = "X";
        public const string Class = "0Z";
        public const string NoSubclass = $"{Class}000";
        public const string MaximumNumberOfStackedDiagnosticsAreasExceeded = $"{Class}001";
        public const string StackedDiagnosticsAccessedWithoutActiveHandler = $"{Class}002";
    }
    
    public static class XqueryError
    {
        public const string Category = "X";
        public const string Class = "10";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class CaseNotFoundForCaseStatement
    {
        public const string Category = "X";
        public const string Class = "20";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class CardinalityViolation
    {
        public const string Category = "X";
        public const string Class = "21";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class DataException
    {
        public const string Category = "X";
        public const string Class = "22";
        public const string NoSubclass = $"{Class}000";
        public const string StringDataRightTruncation = $"{Class}001";
        public const string NullValueNoIndicatorParameter = $"{Class}002";
        public const string NumericValueOutOfRange = $"{Class}003";
        public const string NullValueNotAllowed = $"{Class}004";
        public const string ErrorInAssignment = $"{Class}005";
        public const string InvalidIntervalFormat = $"{Class}006";
        public const string InvalidDatetimeFormat = $"{Class}007";
        public const string DatetimeFieldOverflow = $"{Class}008";
        public const string InvalidTimeZoneDisplacementValue = $"{Class}009";
        public const string EscapeCharacterConflict = $"{Class}00B";
        public const string InvalidUseOfEscapeCharacter = $"{Class}00C";
        public const string InvalidEscapeOctet = $"{Class}00D";
        public const string NullValueInArrayTarget = $"{Class}00E";
        public const string ZeroLengthCharacterString = $"{Class}00F";
        public const string MostSpecificTypeMismatch = $"{Class}00G";
        public const string SequenceGeneratorLimitExceeded = $"{Class}00H";
        public const string NonidenticalNotationsWithTheSameName = $"{Class}00J";
        public const string NonidenticalUnparsedEntitiesWithTheSameName = $"{Class}00K";
        public const string NotAnXmlDocument = $"{Class}00L";
        public const string InvalidXmlDocument = $"{Class}00M";
        public const string InvalidXmlContent = $"{Class}00N";
        public const string IntervalValueOutOfRange = $"{Class}00P";
        public const string MultisetValueOverflow = $"{Class}00Q";
        public const string XmlValueOverflow = $"{Class}00R";
        public const string InvalidComment = $"{Class}00S";
        public const string InvalidProcessingInstruction = $"{Class}00T";
        public const string NotAnXqueryDocumentNode = $"{Class}00U";
        public const string InvalidXqueryContextItem = $"{Class}00V";
        public const string XquerySerializationError = $"{Class}00W";
        public const string InvalidIndicatorParameterValue = $"{Class}010";
        public const string SubstringError = $"{Class}011";
        public const string DivisionByZero = $"{Class}012";
        public const string InvalidPrecedingOrFollowingSizeInWindowFunction = $"{Class}013";
        public const string InvalidArgumentForNtileFunction = $"{Class}014";
        public const string IntervalFieldOverflow = $"{Class}015";
        public const string InvalidArgumentForNthValueFunction = $"{Class}016";
        public const string InvalidDataSpecifiedForDatalink = $"{Class}017";
        public const string InvalidCharacterValueForCast = $"{Class}018";
        public const string InvalidEscapeCharacter = $"{Class}019";
        public const string NullArgumentPassedToDatalinkConstructor = $"{Class}01A";
        public const string InvalidRegularExpression = $"{Class}01B";
        public const string NullRowNotPermittedInTable = $"{Class}01C";
        public const string DatalinkValueExceedsMaximumLength = $"{Class}01D";
        public const string InvalidArgumentForNaturalLogarithm = $"{Class}01E";
        public const string InvalidArgumentForPowerFunction = $"{Class}01F";
        public const string InvalidArgumentForWidthBucketFunction = $"{Class}01G";
        public const string InvalidRowVersion = $"{Class}01H";
        public const string XquerySequenceCannotBeValidated = $"{Class}01J";
        public const string XqueryDocumentNodeCannotBeValidated = $"{Class}01K";
        public const string NoXmlSchemaFound = $"{Class}01L";
        public const string ElementNamespaceNotDeclared = $"{Class}01M";
        public const string GlobalElementNotDeclared = $"{Class}01N";
        public const string NoXmlElementWithTheSpecifiedQname = $"{Class}01P";
        public const string NoXmlElementWithTheSpecifiedNamespace = $"{Class}01Q";
        public const string ValidationFailure = $"{Class}01R";
        public const string InvalidQueryRegularExpression = $"{Class}01S";
        public const string InvalidQueryOptionFlag = $"{Class}01T";
        public const string AttemptToReplaceAZeroLengthString = $"{Class}01U";
        public const string InvalidQueryReplacementString = $"{Class}01V";
        public const string InvalidRowCountInFetchFirstClause = $"{Class}01W";
        public const string InvalidRowCountInResultOffsetClause = $"{Class}01X";
        public const string CharacterNotInRepertoire = $"{Class}021";
        public const string IndicatorOverflow = $"{Class}022";
        public const string InvalidParameterValue = $"{Class}023";
        public const string UnterminatedCString = $"{Class}024";
        public const string InvalidEscapeSequence = $"{Class}025";
        public const string StringDataLengthMismatch = $"{Class}026";
        public const string TrimError = $"{Class}027";
        public const string NoncharacterInUcsString = $"{Class}029";
        public const string NullValueInFieldReference = $"{Class}02A";
        public const string NullValueSubstitutedForMutatorSubjectParameter = $"{Class}02D";
        public const string ArrayElementError = $"{Class}02E";
        public const string ArrayDataRightTruncation = $"{Class}02F";
        public const string InvalidRepeatArgumentInASampleClause = $"{Class}02G";
        public const string InvalidSampleSize = $"{Class}02H";
    }

    public static class IntegrityConstraintViolation
    {
        public const string Category = "X";
        public const string Class = "23";
        public const string NoSubclass = $"{Class}000";
        public const string RestrictViolation = $"{Class}001";
    }

    public static class InvalidCursorState
    {
        public const string Category = "X";
        public const string Class = "24";
        public const string NoSubclass = $"{Class}000";
    }

    public static class InvalidTransactionState
    {
        public const string Category = "X";
        public const string Class = "25";
        public const string NoSubclass = $"{Class}000";
        public const string ActiveSqlTransaction = $"{Class}001";
        public const string BranchTransactionAlreadyActive = $"{Class}002";
        public const string InappropriateAccessModeForBranchTransaction = $"{Class}003";
        public const string InappropriateIsolationLevelForBranchTransaction = $"{Class}004";
        public const string NoActiveSqlTransactionForBranchTransaction = $"{Class}005";
        public const string ReadOnlySqlTransaction = $"{Class}006";
        public const string SchemaAndDataStatementMixingNotSupported = $"{Class}007";
        public const string HeldCursorRequiresSameIsolationLevel = $"{Class}008";
    }

    public static class InvalidSqlStatementName
    {
        public const string Category = "X";
        public const string Class = "26";
        public const string NoSubclass = $"{Class}000";
    }

    public static class TriggeredDataChangeViolation
    {
        public const string Category = "X";
        public const string Class = "27";
        public const string NoSubclass = $"{Class}000";
        public const string ModifyTableModifiedByDataChangeDeltaTable = $"{Class}001";
    }
    
    public static class InvalidAuthorizationSpecification
    {
        public const string Category = "X";
        public const string Class = "28";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class DependentPrivilegeDescriptorsStillExist
    {
        public const string Category = "X";
        public const string Class = "2B";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidCharacterSetName
    {
        public const string Category = "X";
        public const string Class = "2C";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidTransactionTermination
    {
        public const string Category = "X";
        public const string Class = "2D";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidConnectionName
    {
        public const string Category = "X";
        public const string Class = "2E";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class SqlRoutineException
    {
        public const string Category = "X";
        public const string Class = "2F";
        public const string NoSubclass = $"{Class}000";
        public const string ModifyingSqlDataNotPermitted = $"{Class}002";
        public const string ProhibitedSqlStatementAttempted = $"{Class}003";
        public const string ReadingSqlDataNotPermitted = $"{Class}004";
        public const string FunctionExecutedNoReturnStatement = $"{Class}005";
    }
    
    public static class InvalidCollationName
    {
        public const string Category = "X";
        public const string Class = "2H";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidSqlStatementIdentifier
    {
        public const string Category = "X";
        public const string Class = "30";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidSqlDescriptorName
    {
        public const string Category = "X";
        public const string Class = "33";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidCursorName
    {
        public const string Category = "X";
        public const string Class = "34";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidConditionNumber
    {
        public const string Category = "X";
        public const string Class = "35";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class CursorSensitivityException
    {
        public const string Category = "X";
        public const string Class = "36";
        public const string NoSubclass = $"{Class}000";
        public const string RequestRejected = $"{Class}001";
        public const string RequestFailed = $"{Class}002";
    }
    
    public static class ExternalRoutineException
    {
        public const string Category = "X";
        public const string Class = "38";
        public const string NoSubclass = $"{Class}000";
        public const string ContainingSqlNotPermitted = $"{Class}001";
        public const string ModifyingSqlDataNotPermitted = $"{Class}002";
        public const string ProhibitedSqlStatementAttempted = $"{Class}003";
        public const string ReadingSqlDataNotPermitted = $"{Class}004";
    }
    
    public static class ExternalRoutineInvocationException
    {
        public const string Category = "X";
        public const string Class = "39";
        public const string NoSubclass = $"{Class}000";
        public const string NullValueNotAllowed = $"{Class}004";
    }
    
    public static class SavepointException
    {
        public const string Category = "X";
        public const string Class = "3B";
        public const string NoSubclass = $"{Class}000";
        public const string InvalidSpecification = $"{Class}001";
        public const string TooMany = $"{Class}002";
    }
    
    public static class AmbiguousCursorName
    {
        public const string Category = "X";
        public const string Class = "3C";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidCatalogName
    {
        public const string Category = "X";
        public const string Class = "3D";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class InvalidSchemaName
    {
        public const string Category = "X";
        public const string Class = "3F";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class TransactionRollback
    {
        public const string Category = "X";
        public const string Class = "40";
        public const string NoSubclass = $"{Class}000";
        public const string SerializationFailure = $"{Class}001";
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public const string IntegrityConstraintViolation = $"{Class}002";
        public const string StatementCompletionUnknown = $"{Class}003";
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public const string TriggeredActionException = $"{Class}004";
    }
    
    public static class SyntaxErrorOrAccessRuleViolation
    {
        public const string Category = "X";
        public const string Class = "42";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class WithCheckOptionViolation
    {
        public const string Category = "X";
        public const string Class = "44";
        public const string NoSubclass = $"{Class}000";
    }
    
    public static class UnhandledUserDefinedException
    {
        public const string Category = "X";
        public const string Class = "45";
        public const string NoSubclass = $"{Class}000";
    }

    public static class OlbSpecificLanguage
    {
        public const string Category = "X";
        public const string Class = "46";
        public const string NoSubclass = $"{Class}000";
        public const string InvalidUrl = $"{Class}001";
        public const string InvalidJarName = $"{Class}002";
        public const string InvalidClassDeletion = $"{Class}003";
        public const string InvalidReplacement = $"{Class}005";
        public const string AttemptToReplaceUninstalledJar = $"{Class}00A";
        public const string AttemptToRemoveUninstalledJar = $"{Class}00B";
        public const string InvalidJarRemoval = $"{Class}00C";
        public const string InvalidPath = $"{Class}00D";
        public const string SelfReferencingPath = $"{Class}00E";
        public const string InvalidJarNameInPath = $"{Class}102";
        public const string UnresolvedClassName = $"{Class}103";
        public const string UnsupportedFeature = $"{Class}110";
        public const string InvalidClassDeclaration = $"{Class}120";
        public const string InvalidColumnName = $"{Class}121";
        public const string InvalidNumberOfColumns = $"{Class}122";
        public const string InvalidProfileState = $"{Class}130";
    }

    public static class DataLinkException
    {
        public const string Category = "X";
        public const string Class = "HW";
        public const string NoSubclass = $"{Class}000";
        public const string ExternalFileNotLinked = $"{Class}001";
        public const string ExternalFileAlreadyLinked = $"{Class}002";
        public const string ReferencedFileDoesNotExist = $"{Class}003";
        public const string InvalidWriteToken = $"{Class}004";
        public const string InvalidDatalinkConstruction = $"{Class}005";
        public const string InvalidWritePermissionForUpdate = $"{Class}006";
        public const string ReferencedFileNotValid = $"{Class}007";
    }

    public static class FdwSpecificCondition
    {
        public const string Category = "X";
        public const string Class = "HV";
        public const string NoSubclass = $"{Class}000";
        public const string MemoryAllocationError = $"{Class}001";
        public const string DynamicParameterValueNeeded = $"{Class}002";
        public const string InvalidDataType = $"{Class}004";
        public const string ColumnNameNotFound = $"{Class}005";
        public const string InvalidDataTypeDescriptors = $"{Class}006";
        public const string InvalidColumnName = $"{Class}007";
        public const string InvalidColumnNumber = $"{Class}008";
        public const string InvalidUseOfNullPointer = $"{Class}009";
        public const string InvalidStringFormat = $"{Class}00A";
        public const string InvalidHandle = $"{Class}00B";
        public const string InvalidOptionIndex = $"{Class}00C";
        public const string InvalidOptionName = $"{Class}00D";
        public const string OptionNameNotFound = $"{Class}00J";
        public const string ReplyHandle = $"{Class}00K";
        public const string UnableToCreateExecution = $"{Class}00L";
        public const string UnableToCreateReply = $"{Class}00M";
        public const string UnableToEstablishConnection = $"{Class}00N";
        public const string NoSchemas = $"{Class}00P";
        public const string SchemaNotFound = $"{Class}00Q";
        public const string TableNotFound = $"{Class}00R";
        public const string FunctionSequenceError = $"{Class}010";
        public const string LimitOnNumberOfHandlesExceeded = $"{Class}014";
        public const string InconsistentDescriptorInformation = $"{Class}021";
        public const string InvalidAttributeValue = $"{Class}024";
        public const string InvalidStringLengthOrBufferLength = $"{Class}090";
        public const string InvalidDescriptorFieldIdentifier = $"{Class}091";
    }

    public static class CliSpecificCondition
    {
        public const string Category = "X";
        public const string Class = "HY";
        public const string NoSubclass = $"{Class}000";
        // public const string DynamicParameterValueNeeded = $"{Class}N/A";
        // public const string InvalidHandle = $"{Class}N/A";
        public const string MemoryAllocationError = $"{Class}001";
        public const string InvalidDataTypeInApplicationDescriptor = $"{Class}003";
        public const string InvalidDataType = $"{Class}004";
        public const string AssociatedStatementIsNotPrepared = $"{Class}007";
        public const string OperationCanceled = $"{Class}008";
        public const string InvalidUseOfNullPointer = $"{Class}009";
        public const string FunctionSequenceError = $"{Class}010";
        public const string AttributeCannotBeSetNow = $"{Class}011";
        public const string InvalidTransactionOperationCode = $"{Class}012";
        public const string MemoryManagementError = $"{Class}013";
        public const string LimitOnNumberOfHandlesExceeded = $"{Class}014";
        public const string InvalidUseOfAutomaticallyAllocatedDescriptorHandle = $"{Class}017";
        public const string ServerDeclinedTheCancellationRequest = $"{Class}018";
        public const string NonStringDataCannotBeSentInPieces = $"{Class}019";
        public const string AttemptToConcatenateANullValue = $"{Class}020";
        public const string InconsistentDescriptorInformation = $"{Class}021";
        public const string InvalidAttributeValue = $"{Class}024";
        public const string NonStringDataCannotBeUsedWithStringRoutine = $"{Class}055";
        public const string InvalidStringLengthOrBufferLength = $"{Class}090";
        public const string InvalidDescriptorFieldIdentifier = $"{Class}091";
        public const string InvalidAttributeIdentifier = $"{Class}092";
        public const string InvalidDatalinkValue = $"{Class}093";
        public const string InvalidFunctionidSpecified = $"{Class}095";
        public const string InvalidInformationType = $"{Class}096";
        public const string ColumnTypeOutOfRange = $"{Class}097";
        public const string ScopeOutOfRange = $"{Class}098";
        public const string NullableTypeOutOfRange = $"{Class}099";
        public const string InvalidRetrievalCode = $"{Class}103";
        public const string InvalidLengthprecisionValue = $"{Class}104";
        public const string InvalidParameterMode = $"{Class}105";
        public const string InvalidFetchOrientation = $"{Class}106";
        public const string RowValueOutOfRange = $"{Class}107";
        public const string InvalidCursorPosition = $"{Class}108";
        public const string OptionalFeatureNotImplemented = $"{Class}C00";
    }
}