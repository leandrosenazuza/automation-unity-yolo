from ultralytics import YOLO
from flask import Flask, request, jsonify  # type: ignore
import os

app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def predict():
    data = request.json
    image_path = data['image_path']
    model = YOLO("best.pt")
    results = model.predict(source=image_path)

    response = []
    for result in results:
        for box in result.boxes:
            if model.names[box.cls.item()] == 'reator':
                response.append({
                    'class_name': model.names[box.cls.item()],
                    'bbox': box.xyxy.tolist()
                })

    return jsonify(response)

    if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)