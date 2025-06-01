# Samples and Examples

This repo contains code illustrating how to interact with the Worth API. 
Each directory will host a small sample project illustrating a use case inspired from customer needs. 

_NOTE_: please note that each of these projects are created for demo purposes only and are not production-ready.
These projects are intended to speed up developers working with our API. 

## Bulk Batcher (C#/.NET)
The Bulk Batcher is a small .NET 9.0 project that demonstrates the following:
- consuming a JSON array representing business payloads
- deserializing the elements to a C# model, based on the required paramters in our [API docs](https://docs.worthai.com/api-reference/add-or-update-business/add-business#body-total-accounts-payable)
- batching the elements into chunked lists of 5 elements
- sending the lists to the API and awaiting results
