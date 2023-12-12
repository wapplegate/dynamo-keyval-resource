# Dynamo Keyval Resource

This resource stores key/value pairs similar to the "keyval" resource, however, it persists them in DynamoDB so they are not lost when pipelines are destroyed. This also allows values set in one pipeline to be used in another.

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

### Set a Key & Value

If you need to set the key/value pair from a job you can do it as follows:

```yml
- name: build
  public: true
  plan:
    - in_parallel:
      - get: rust-build-image
      - get: version
        params: { pre: RC }
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

### Get a Key & Value

If you need to get the value for the configured key you can do it as follows:

```yml
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

After your pipeline runs a few times, the DynamoDB table might look something like this:

| application | created      | value |
|-------------|--------------|-------|
| `my-app`    | `12/12/2023` | `1.2` |
| `my-app`    | `12/11/2023` | `1.1` |
| `my-app`    | `12/10/2023` | `1.0` |

## Development

The following articles provide insight into how to create a custom Concourse resource:

[Implementing Resource Types](https://concourse-ci.org/implementing-resource-types.html)

[Writing a Custom Concourse Resource Overview](https://medium.com/@alexander.jansing/writing-a-custom-concourse-resource-overview-1ed6d2983e39)

[Developing a Custom Concourse Resource](https://tanzu.vmware.com/content/blog/developing-a-custom-concourse-resource)
