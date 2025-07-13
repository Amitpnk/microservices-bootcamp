```
winget install -e --id Microsoft.AzureCLI
```

- Login Azure
```
az login
```

- Create Resource Group
```
az group create --name rg-eventick-dev-001 --location eastus2 --tags 'Project=Eventick' 'Environment=Development'
```

```
az consumption budget create --amount 100 --time-grain Monthly --start-date $(date +%Y-%m-01) --end-date 2025-12-31 --category Cost --resource-group rg-eventick-dev-001 --budget-name "Eventick Dev Monthly Budget"
```

- Create Service Bus Name Space
```
az servicebus namespace create \
  --name sb-eventick-dev-001 \
  --resource-group rg-eventick-dev-001 \
  --location eastus2 \
  --sku Standard \
  --tags 'Project=Eventick' 'Environment=Development'
```

- Create Service Bus Topics

```
# Create checkoutmessage topic
az servicebus topic create \
  --name checkoutmessage \
  --namespace-name sb-eventick-dev-001 \
  --resource-group rg-eventick-dev-001 \
  --default-message-time-to-live P14D \
  --enable-duplicate-detection true \
  --duplicate-detection-history-time-window PT10M

# Create orderpaymentrequestmessage topic
az servicebus topic create \
  --name orderpaymentrequestmessage \
  --namespace-name sb-eventick-dev-001 \
  --resource-group rg-eventick-dev-001 \
  --default-message-time-to-live P14D \
  --enable-duplicate-detection true \
  --duplicate-detection-history-time-window PT10M

# Create orderpaymentupdatedmessage topic
az servicebus topic create \
  --name orderpaymentupdatedmessage \
  --namespace-name sb-eventick-dev-001 \
  --resource-group rg-eventick-dev-001 \
  --default-message-time-to-live P14D \
  --enable-duplicate-detection true \
  --duplicate-detection-history-time-window PT10M
```

- Create Subscriptions for Topics
```
az servicebus topic subscription create \
  --resource-group rg-eventick-dev-001 \
  --namespace-name sb-eventick-dev-001 \
  --topic-name checkoutmessage \
  --name sbCheckoutMessageSubscription

az servicebus topic subscription create \
  --resource-group rg-eventick-dev-001 \
  --namespace-name sb-eventick-dev-001 \
  --topic-name orderpaymentrequestmessage \
  --name sbOrderPaymentRequestSubscription 

az servicebus topic subscription create \
  --resource-group rg-eventick-dev-001 \
  --namespace-name sb-eventick-dev-001 \
  --topic-name orderpaymentupdatedmessage \
  --name sbOrderPaymentUpdatedSubscription
```

- Verification Commands
```
az servicebus topic list --namespace-name sb-eventick-dev-001 --resource-group rg-eventick-dev-001

az servicebus topic show --name checkoutmessage --namespace-name sb-eventick-dev-001 --resource-group rg-eventick-dev-001
az servicebus topic show --name orderpaymentrequestmessage --namespace-name sb-eventick-dev-001 --resource-group rg-eventick-dev-001
az servicebus topic show --name orderpaymentupdatedmessage --namespace-name sb-eventick-dev-001 --resource-group rg-eventick-dev-001
```

- Get connection primary connection string of azure service bus
```
az servicebus namespace authorization-rule keys list \
  --resource-group rg-eventick-dev-001 \
  --namespace-name sb-eventick-dev-001 \
  --name RootManageSharedAccessKey \
  --query primaryConnectionString \
  --output tsv
```



-- Clean Up
az group delete --name rg-eventick-dev-001 --yes