# Dynamo Keyval Resource

This resource stores key/value pairs similar to the keyval resource, however, it persists them in dynamo so they are not lost when pipelines are created and destroyed. This would allow values set in one pipeline to be used in another.

## Usage

```yml
resources:
  - name: artifacts
    type: dynamo-keyval-resource
    icon: package-variant-closed
    source:
      table_name:    packages-table       # Name of the dynamo table to store records in.
      partition_key: application          # The name of the partition key of the dynamo table.
      key:           feature-management   # The value of the partition key.
      access_key:    ((aws.access-key))   # Your AWS access key.
      secret_key:    ((aws.secret-key))   # Your AWS secret key.
      region:        us-east-1            # The region where the dynamo table is deployed.

resource_types:
  - name: dynamo-keyval-resource
    type: registry-image
    source: 
      repository: wapplegate/dynamo-keyval-resource
      tag: latest
```

## TODO

- Rename the individual projects.
- Add a testing project.
- Write unit and integration tests.

## Development

This repository provides an example of how to create a concourse resource using .NET and C#. The following articles provided insight into how to create a custom concourse resource.

```text
https://concourse-ci.org/implementing-resource-types.html
https://medium.com/@alexander.jansing/writing-a-custom-concourse-resource-overview-1ed6d2983e39
https://github.com/apjansing/mongo_resource#how-the-resource-is-written
https://tanzu.vmware.com/content/blog/developing-a-custom-concourse-resource
https://github.com/BrianMMcClain/mysql-concourse-resource/tree/master
```
