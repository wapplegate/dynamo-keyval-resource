# Dynamo Keyval Resource

This resource stores key/value pairs similar to the "keyval" resource, however, it persists the key/value pairs in DynamoDB so they are not lost when pipelines are destroyed. This also allows values set in one pipeline to be used in another. This resource is written in C# and uses .NET 8 AOT so the image is as small as possible.

[![dockeri.co](https://dockerico.blankenship.io/image/wapplegate/dynamo-keyval-resource)](https://hub.docker.com/r/wapplegate/dynamo-keyval-resource)

https://hub.docker.com/r/wapplegate/dynamo-keyval-resource/tags

## Behavior

### Check

The `check` script of this resource queries all items from the DynamoDB table using the given partition key and returns all items that match.

### In

The `in` script of this resource retrieves the latest key and value returned from the `check` and makes it available in your task.

### Out

The `out` script of this resource creates a new item in the DynamoDB table and sets the partition key, created, and value fields.

## Usage

### Configure Resource and Resource Type

```yml
resources:
  - name: artifacts
    type: dynamo-keyval-resource
    icon: package-variant-closed
    source:
      table_name:    packages-table     # Name of the dynamo table to store records in.
      partition_key: application        # The name of the partition key of the dynamo table.
      key:           my-app             # The value of the partition key.
      access_key:    ((aws.access-key)) # Your AWS access key.
      secret_key:    ((aws.secret-key)) # Your AWS secret key.
      region:        us-east-1          # The region where the dynamo table is deployed.

resource_types:
  - name: dynamo-keyval-resource
    type: registry-image
    source: 
      repository: wapplegate/dynamo-keyval-resource
      tag: latest
```

### Setting a Value

To set the key/value pair from a job you can do the following:

```yml
jobs:
  - name: build
    plan:
      - in_parallel:
        - get: rust-build-image
        - get: version
      - task: build
        image: rust-build-image
        config:
          platform: linux
          inputs:
            - name: version
          run:
            path: bash
            args:
              - -exc
              - |
                export ROOT_DIR=$PWD
                version=$(cat ${ROOT_DIR}/version/number)
                echo $version;
                cd artifacts
                echo "${version}" > version.txt # Creates and sets a file with the version number.
          outputs:
            - name: artifacts
      - put: artifacts
        inputs:
          - artifacts
```

### Getting a Value

To get the value for the configured key you can do the following:

```yml
jobs:
  - name: deploy
    plan:
      - in_parallel:
        - get: rust-build-image
        - get: artifacts
      - task: deploy
        image: rust-build-image
        config:
          platform: linux
          inputs: 
            - name: artifacts
          run:
            path: bash
            args:
              - -exc
              - |
                export ROOT_DIR=$PWD
                artifact=$(cat ${ROOT_DIR}/artifacts/version.txt)
                echo "Artifact is ${artifact}"
```

After the build job in the example pipeline above runs a few times, the DynamoDB table will have values similar to what is shown below:

| application | created               | value         |
|-------------|-----------------------|---------------|
| `my-app`    | `12/12/2023 18:02:04` | `package-1.2` |
| `my-app`    | `12/11/2023 12:06:39` | `package-1.1` |
| `my-app`    | `12/10/2023 13:21:01` | `package-1.0` |

You can then use these values in other jobs or pipelines.

## Development

The following articles provide insight into how to create a custom Concourse resource:

[Implementing Resource Types](https://concourse-ci.org/implementing-resource-types.html)

[Writing a Custom Concourse Resource Overview](https://medium.com/@alexander.jansing/writing-a-custom-concourse-resource-overview-1ed6d2983e39)

[Developing a Custom Concourse Resource](https://tanzu.vmware.com/content/blog/developing-a-custom-concourse-resource)
