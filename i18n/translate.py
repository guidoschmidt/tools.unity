#!/usr/bin/env python
# -*- coding: utf-8 -*-
import requests
import os

API_URL = "https://api.openai.com/v1/chat/completions"
AUTH_TOKEN= os.environ("GPT_AUTH_TOKEN")

def read_reference_csv():
    contents = ""
    with open("reference.csv", "r", encoding="utf8") as csvfile:
        contents = csvfile.read()
        csvfile.close()
    return contents


def get_translation():
    headers = {
        "Authorization": f"Bearer {AUTH_TOKEN}",
        "Content-Type": "application/json",
    }
    payload = {
        "model": "gpt-4-1106-preview",
        "messages": [
            {
                "role": "system",
                "content": "You are a translator"
            },
            {
                "role": "user",
                "content": f"{prompt}\n\n{read_reference_csv()}"
            }
        ]
    }
    r = requests.post(API_URL, json=payload, headers=headers)
    content = r.json()["choices"][0]["message"]["content"]
    with open(f"{lang_code}.csv", "w", encoding="utf8") as outfile:
        outfile.write(content)
        outfile.close()


langs = [
    {
        "language": "French",
        "lang_code": "fr"
    },
    {
        "language": "Spanish",
        "lang_code": "es"
    },
    {
        "language": "Portuguese",
        "lang_code": "pt"
    },
    {
        "language": "Italian",
        "lang_code": "it"
    },
    {
        "language": "German",
        "lang_code": "de"
    },
    {
        "language": "Japanese",
        "lang_code": "ja"
    },
    {
        "language": "Chinese (Simplified)",
        "lang_code": "zh"
    },
    {
        "language": "Korean",
        "lang_code": "ko"
    }
]

for lang in langs:
   language = lang["language"]
   lang_code = lang["lang_code"]
   prompt = f"""Translate the following CSV by translating column labelled "English(en)" to {language} and put it into a new column labelled "{language}({lang_code})". Please only contain the CSV data in the answer:"""
   print(f"\n>>> Translating to {language}")
   get_translation()
