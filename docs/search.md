

### i. Returning Book with CountryCode = HK or US

```bash
curl -X POST "http://localhost:5137/api/books/search" \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "US",
    "filter": {
      "operator": "Or",
      "conditions": [
        { "field": "CountryCode", "operator": "Equals", "value": "HK" },
        { "field": "CountryCode", "operator": "Equals", "value": "US" }
      ]
    }
  }'

```

---

### ii. Category = 2 and (CountryCode = HK or US)

```bash
curl -X POST "http://localhost:5137/api/books/search" \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "US",
    "filter": {
      "operator": "And",
      "conditions": [
        { "field": "Category", "operator": "Equals", "value": "2" }
      ],
      "filters": [
        {
          "operator": "Or",
          "conditions": [
            { "field": "CountryCode", "operator": "Equals", "value": "HK" },
            { "field": "CountryCode", "operator": "Equals", "value": "US" }
          ]
        }
      ]
    }
  }'

```

---

### iii. Name containing "Wiki" and CountryCode = Not US

```bash
curl -X POST "http://localhost:5137/api/books/search" \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "US",
    "filter": {
      "operator": "And",
      "conditions": [
        { "field": "Name", "operator": "Contains", "value": "Wiki" },
        { "field": "CountryCode", "operator": "NotEquals", "value": "US" }
      ]
    }
  }'

```