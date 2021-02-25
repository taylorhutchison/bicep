
output myStr string = 'hello'

output myInt int = 7
output myOtherInt int = 20 / 13 + 80 % -4

output myBool bool = !false
output myOtherBool bool = true

output suchEmpty array = [
]

output suchEmpty2 object = {
}

output obj object = {
  a: 'a'
  b: 12
  c: true
  d: null
  list: [
    1
    2
    3
    null
    {
    }
  ]
  obj: {
    nested: [
      'hello'
    ]
  }
}

output myArr array = [
  'pirates'
  'say'
   false ? 'arr2' : 'arr'
]

output rgLocation string = resourceGroup().location

output isWestUs bool = resourceGroup().location != 'westus' ? false : true

output expressionBasedIndexer string = {
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[4:31) Variable secondaryKeyIntermediateVar. Type: any. Declaration start char: 0, length: 106

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
output secondaryKey string = secondaryKeyIntermediateVar

var varWithOverlappingOutput = 'hello'
//@[4:28) Variable varWithOverlappingOutput. Type: 'hello'. Declaration start char: 0, length: 38
param paramWithOverlappingOutput string
//@[6:32) Parameter paramWithOverlappingOutput. Type: string. Declaration start char: 0, length: 39

output varWithOverlappingOutput string = varWithOverlappingOutput
output paramWithOverlappingOutput string = paramWithOverlappingOutput
